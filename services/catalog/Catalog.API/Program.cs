using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Contexts;
using Catalog.Infrastructure.Repositories;
using Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog(Logging.ConfigureLogging);
builder.Services.AddControllers();

// Add Idnetity Server Authentication (Duende)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://host.docker.internal:9009";
        options.RequireHttpsMetadata = true;

        // i have received ACCESS TOKEN should i grant access to the API or not ?
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://host.docker.internal:9009",
            ValidateAudience = true,
            ValidAudience = "Catalog",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
        // Add this to docker to host communication .
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed.");
                Console.WriteLine($"Exception: {context.Exception.Message}");
                Console.WriteLine($"Authority: {options.Authority}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "catalogapi.read");
    });
});
   
builder.Services.AddAutoMapper(cfg => { }, typeof(ProductMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(),
    Assembly.GetAssembly(typeof(GetProductsByIdQuery))));

builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();
builder.Services.AddScoped<ITypeRepository, ProductRepository>();

var userPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(userPolicy));
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
