var builder = DistributedApplication.CreateBuilder(args);
// Aspire to use the default service settings for the application, which includes configuring logging, configuration, and other essential services.
var sqlPassword = builder.AddParameter("sql-password", secret: true);
var pgPassword = builder.AddParameter("pg-password", secret: true);

var catalogMongo = builder.AddMongoDB("catalogdb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();
var catalogDb = catalogMongo.AddDatabase("catalog-db", databaseName: "CatalogDb");

var basketRedis = builder.AddRedis("basketdb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var discountPostgres = builder.AddPostgres("discountdb", password: pgPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin();
var discountDb = discountPostgres.AddDatabase("discount-db", databaseName: "DiscountDb");

var orderSql = builder.AddSqlServer("orderdb", password: sqlPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();
var orderDb = orderSql.AddDatabase("OrderDb2");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var kibana = builder.AddContainer("kibana", "docker.elastic.co/kibana/kibana", "8.14.3")
    .WithEnvironment("ELASTICSEARCH_URL", elasticsearch.GetEndpoint("http"))
    .WithHttpEndpoint(targetPort: 5601, name: "http")
    .WaitFor(elasticsearch);

var identity = builder.AddProject<Projects.eShop_Identity>("identity")
    .WithHttpsEndpoint(port: 9009, name: "https")
    .WithExternalHttpEndpoints();

var discountApi = builder.AddProject<Projects.Discount_API>("discount-api")
    .WithReference(discountDb).WaitFor(discountDb)
    .WithReference(elasticsearch)
    .WithEnvironment("Auth__Authority", identity.GetEndpoint("https"))
    .WithHttpEndpoint(port: 8084, name: "http");

var catalogApi = builder.AddProject<Projects.Catalog_API>("catalog-api")
    .WithReference(catalogDb).WaitFor(catalogMongo)
    .WithReference(elasticsearch)
    .WithEnvironment("Auth__Authority", identity.GetEndpoint("https"))
    .WithHttpEndpoint(port: 8080, name: "http");

var basketApi = builder.AddProject<Projects.Basket_API>("basket-api")
    .WithReference(basketRedis).WaitFor(basketRedis)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithReference(discountApi)
    .WithEnvironment("Auth__Authority", identity.GetEndpoint("https"))
    .WithHttpEndpoint(port: 8082, name: "http");

var orderingApi = builder.AddProject<Projects.Ordering_API>("ordering-api")
    .WithReference(orderDb).WaitFor(orderSql)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 8086, name: "http");

var gateway = builder.AddProject<Projects.Ocelot_APIGateway>("gateway")
    .WithReference(catalogApi).WithReference(basketApi)
    .WithReference(discountApi).WithReference(orderingApi)
    .WaitFor(catalogApi).WaitFor(basketApi)
    .WaitFor(discountApi).WaitFor(orderingApi)
    .WithEnvironment("Auth__Authority", identity.GetEndpoint("https"))
    .WithExternalHttpEndpoints();

builder.Build().Run();