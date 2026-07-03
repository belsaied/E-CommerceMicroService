using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Contexts;
using Catalog.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAutoMapper(cfg => { }, typeof(ProductMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();
builder.Services.AddScoped<ITypeRepository, ProductRepository>();

builder.Services.AddApiVersioning(options =>
{
    // return available versions in response header
    options.ReportApiVersions = true;
    // Assume default version when not specified
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Default API Version
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "Catalog API for E-Commerce Microservice",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "belal Saied",
            Email = "bellllyelnagggar225@gmail.com",
            Url = new Uri("https://github.com/belsaied")
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
