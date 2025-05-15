
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.WebAPI.Middleware;
using MyAppDemo.WebAPI.Services;

namespace MyAppDemo.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddControllers();
        
        builder.Services.AddAuthorization();

        // Add Entity Framework Core context
        var connectionString = Environment.GetEnvironmentVariable("MSPPWebAPI2025") 
            ?? builder.Configuration.GetConnectionString("MSPPWebAPI2025");

        Console.WriteLine($"Connection string: {connectionString}");

        builder.Services.AddDbContext<WebAPIDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                );
            }));



        builder.Services.AddScoped<IGitHubService, GitHubService>();

        builder.Services.AddAuthentication("ApiKeyScheme")
            .AddScheme<AuthenticationSchemeOptions, DummyApiKeyAuthenticationHandler>("ApiKeyScheme", null);


        // HttpClient dependency injection
        builder.Services.AddHttpClient<IWebhookService, WebhookService>();
        builder.Services.AddHttpClient<IPerplexityService, PerplexityService>();
        builder.Services.AddScoped<IApiKeyValidationService, ApiKeyValidationService>();




        // OpenAPI document generation
        builder.Services.AddOpenApi();

        // Add Swagger generation
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Power Platorm Custom Connectors - Teched 2025", Version = "v1" });

            // Adds security definition for API Key
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. X-API-Key: {key}",
                In = ParameterLocation.Header,
                Name = "X-API-Key",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // CORS policy, allow all origins, headers, and methods
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // CORS policy
        app.UseCors("AllowAll");


        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
        }


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Power Platorm Custom Connectors - Teched 2025 V1");
            c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
        });


        // Middleware for authentication via API Key
        app.UseMiddleware<ApiKeyAuthMiddleware>();

        // Logging Middleware
        app.UseMiddleware<LoggingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();



        app.MapControllers();

        app.MapSwagger().RequireAuthorization();

        app.MapGet("/", () => "Hello, TechEd 2025!");
        app.MapGet("/api/hello", () => "Hello from the API!").RequireAuthorization();
        app.MapGet("/api/hello/{name}", (string name) => $"Hello, {name} from the API!").RequireAuthorization();

        app.Run();
    }
}
