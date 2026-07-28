using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
// API versioning.
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
