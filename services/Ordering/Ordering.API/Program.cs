using Common.Logging;
using EventBus.Messages.Common;
using MassTransit;
using MassTransit.MultiBus;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(Logging.ConfigureLogging);


// API versioning
builder.Services.AddApiVersioning(options =>
{
    // return available versions in response header
    options.ReportApiVersions = true;
    // Assume default version when not specified
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Default API Version
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
});
// swagger.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ordering API",
        Version = "v1",
        Description = "Order API for E-Commerce Microservice",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "belal Saied",
            Email = "bellllyelnagggar225@gmail.com",
            Url = new Uri("https://github.com/belsaied")
        }
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfraServices(builder.Configuration);

builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddScoped<BasketOrderingConsumerV2>();
builder.Services.AddMassTransit(config =>
{
    // Mark this as consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.AddConsumer<BasketOrderingConsumerV2>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        // provide the queue name with consumer
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });

        // version 2
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumerV2>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, serviceProvider) =>
{
    var logger = serviceProvider.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
