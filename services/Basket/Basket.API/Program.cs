using Basket.Application.Commands;
using Basket.Application.GrpcServices;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Discount.gRPC.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
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
            ValidAudience = "Basket",
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

builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(cfg => { }, typeof(BasketMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(),
    Assembly.GetAssembly(typeof(CreateShoppingCartCommand))));

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(cfg =>
{
    cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
});

var userPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(userPolicy));
});

// identifying RabbitMQ host address from appsettings.json
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddApiVersioning(options =>
{
    // return available versions in response header
    options.ReportApiVersions = true;
    // Assume default version when not specified
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Default API Version
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Basket API",
        Version = "v1",
        Description = "Basket API for E-Commerce Microservice version 1.0",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "belal Saied",
            Email = "bellllyelnagggar225@gmail.com",
            Url = new Uri("https://github.com/belsaied")
        }
    });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Basket API",
        Version = "v2",
        Description = "Basket API for E-Commerce Microservice Version 2.0",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "belal Saied",
            Email = "belalsaied78@gmail.com",
            Url = new Uri("https://github.com/belsaied")
        }
    });

    options.DocInclusionPredicate((version, desc) =>
    {
        if(!desc.TryGetMethodInfo(out MethodInfo methodInfo)) return false; 

        var versions = methodInfo.DeclaringType?  // get the ApiVersion attribute from the controller
            .GetCustomAttributes(true)
            .OfType<Asp.Versioning.ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        return versions?.Any(v => $"v{v.ToString()}" == version) ?? false;
    });
});

// redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:connectionString");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Basket.API v2");
    });
}
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
