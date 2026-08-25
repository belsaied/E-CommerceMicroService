# E-Commerce Microservices Platform

A distributed, event-driven e-commerce backend built with .NET, designed around Clean Architecture and CQRS at the service level, and orchestrated with .NET Aspire. It splits a typical e-commerce domain (catalog, basket, discounts, ordering, identity) into independently deployable services that communicate synchronously (gRPC) where consistency and low latency matter, and asynchronously (RabbitMQ/MassTransit) where decoupling and resilience matter more.

Repo: https://github.com/belsaied/E-CommerceMicroService

---

## Why this architecture

A monolithic e-commerce app is simple to start with but gets painful fast: the catalog team can't ship independently of the ordering team, a spike in basket traffic can take down checkout, and every service is forced onto the same database technology whether it fits or not.

This project splits the domain along business capability lines, gives each service its own datastore (polyglot persistence), and uses an API Gateway as the single entry point for clients. The trade-off is accepted deliberately: more moving parts and eventual consistency in exchange for independent scaling, independent deployability, and fault isolation.

---

## Services

| Service | Responsibility | Datastore | Notes |
|---|---|---|---|
| **Catalog.API** | Product, brand, and type catalog | MongoDB | Read-heavy service; document store fits the semi-structured product data well |
| **Basket.API** | Shopping cart management | Redis | Cart data is ephemeral/session-like, so an in-memory cache outperforms a relational store here |
| **Discount.API** | Coupon and discount lookup | PostgreSQL | Exposed over **gRPC**, not REST — this is an internal, high-frequency, low-latency call from Basket |
| **Ordering.API** | Order creation and history | SQL Server | Relational integrity matters here (orders, line items, totals) |
| **eShop.Identity** | Authentication/authorization | — | Duende IdentityServer (OIDC/OAuth2) issuing JWTs consumed by every other service |
| **Ocelot.APIGateway** | Single entry point for clients | — | Routes, aggregates, and centralizes auth enforcement for downstream services |

Each business service (Catalog, Basket, Discount, Ordering) follows the same internal Clean Architecture layering:

```
Service.API            → Controllers, composition root (Program.cs), Swagger
Service.Application    → CQRS: Commands, Queries, Handlers (MediatR), Validators (FluentValidation), Mappers (AutoMapper)
Service.Core            → Entities, repository interfaces (no external dependencies)
Service.Infrastructure  → EF Core / Mongo / Postgres repository implementations, DB context, seeding
```

This keeps business rules and use cases independent of any specific database or framework — swapping Catalog's storage from MongoDB to something else wouldn't touch the Application or Core layers.

---

## Architecture & communication patterns

**Synchronous — gRPC (Basket → Discount)**
When an item is priced in the basket, Basket.API calls Discount.API directly over gRPC (`DiscountGrpcService` → `DiscountProtoService`). This is a request/response, need-it-right-now call, so gRPC's binary protocol and strong contract (`discount.proto`) beat REST here on latency and payload size.

**Asynchronous — RabbitMQ + MassTransit (Basket → Ordering)**
Checkout is a different story. When a user checks out, Basket.API doesn't call Ordering.API directly — it publishes a `BasketCheckoutEvent` to RabbitMQ via MassTransit, removes the basket, and returns `202 Accepted` immediately. Ordering.API consumes that event asynchronously and creates the order. This decouples the two services: if Ordering is temporarily down or slow, checkouts still succeed and orders are created once Ordering catches up. The trade-off is eventual consistency — the order doesn't exist the instant checkout returns, only shortly after.

**API Gateway — Ocelot**
All client traffic goes through Ocelot, which routes requests to the right downstream service by path and enforces authentication on sensitive routes (e.g. `CheckoutBasket` requires a valid token via `EShopGatewayAuthSchema`) before the request ever reaches a service.

**Authentication — Duende IdentityServer**
Services don't handle their own user credentials. eShop.Identity issues JWTs (OIDC/OAuth2, including client-credentials flow for service-to-service calls and authorization-code + PKCE for interactive clients). Every downstream API validates the token's issuer, audience, and signature independently — there's no shared session state, which keeps services stateless and horizontally scalable.

**API versioning**
Basket.API and Ordering.API expose both v1 and v2 endpoints side by side (e.g. `CheckoutBasket` v1 vs v2, with a second `BasketCheckoutEventV2`/consumer pair), with Swagger correctly scoping each version's docs via a custom `DocInclusionPredicate`. This let the checkout contract evolve without breaking existing clients.

**Orchestration — .NET Aspire**
Rather than hand-writing every connection string and startup order, the AppHost project declares each resource (MongoDB, Redis, Postgres, SQL Server, RabbitMQ, Elasticsearch/Kibana) and each service's dependency on it (`WithReference`, `WaitFor`), plus service discovery so services find each other by logical name instead of hardcoded hosts/ports. Docker Compose is still provided as a container-only alternative to running the full Aspire stack.

**Resilience**
Every outbound HTTP call goes through .NET's standard resilience handler (`AddStandardResilienceHandler`), giving retries, circuit breaking, and timeouts by default rather than bespoke Polly policies per client.

**Observability**
- OpenTelemetry for tracing, metrics, and structured logging across every service
- Serilog piping logs to Elasticsearch, visualized in Kibana
- `/health` and `/alive` endpoints on every service for container/orchestrator health checks

---

## Business rules worth calling out

- A checkout cannot be submitted without a username, a non-negative total price, an email, a first name, and a last name (enforced via FluentValidation before the command ever reaches a handler).
- The basket is deleted **after** the checkout event is successfully published to RabbitMQ, not before — so a failed publish never leaves the user with an empty cart and no order in flight.
- Pricing shown in the basket reflects the live discount looked up from Discount.API at request time, not a cached/stale price.
- Order creation is driven entirely by the consumed event, not by a direct API call — Ordering.API has no public "create order from basket" endpoint, by design.

---

## Tech stack

**Core**: C# / .NET, ASP.NET Core Web API, .NET Aspire
**Data access**: Entity Framework Core (SQL Server), MongoDB driver, PostgreSQL, StackExchange.Redis
**Messaging**: RabbitMQ, MassTransit
**Sync RPC**: gRPC
**CQRS/mediation**: MediatR
**Validation**: FluentValidation
**Mapping**: AutoMapper
**Auth**: Duende IdentityServer, JWT Bearer
**Gateway**: Ocelot
**Observability**: OpenTelemetry, Serilog, Elasticsearch, Kibana
**Containerization**: Docker, Docker Compose

---

## Running the project

**Option 1 — .NET Aspire (recommended for local development)**
```bash
dotnet run --project E-CommerceMicroService.AppHost
```
This spins up every dependency (Mongo, Redis, Postgres, SQL Server, RabbitMQ, Elasticsearch, Kibana) as containers, wires up service discovery, and launches the Aspire dashboard for logs/traces/metrics across all services.

**Option 2 — Docker Compose**
```bash
docker compose up --build
```

Once running, all traffic goes through the gateway at the port defined in `ocelot.Development.json` (`GlobalConfiguration.BaseUrl`), which fronts Catalog, Basket, Discount, and Ordering.

---

## Trade-offs

| Decision | Benefit | Cost accepted |
|---|---|---|
| Database-per-service | Each service picks the right storage model, independent scaling | No cross-service joins/transactions; more infrastructure to run |
| Async checkout via RabbitMQ | Basket stays responsive even if Ordering is degraded | Eventual consistency — order creation isn't instantaneous |
| gRPC for Basket↔Discount | Low latency, strongly-typed contract | Extra tooling (proto codegen), not human-readable like REST |
| Centralized identity (Duende) | Stateless services, no session sharing needed | Every service takes a hard dependency on Identity being reachable |
| API Gateway (Ocelot) | Single entry point, centralized auth enforcement | Extra network hop; gateway config must stay in sync with routes |
