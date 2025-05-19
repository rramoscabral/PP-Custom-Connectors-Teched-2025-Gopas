
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.WebAPI.Middleware;
using MyAppDemo.WebAPI.Models.Responses;
using MyAppDemo.WebAPI.Services;
using System.Reflection;

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

        // Only for testing purposes
        //Console.WriteLine($"Connection string: {connectionString}");

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

        // Automatically generate OpenAPI documentation for endpoints defined with MapGet, MapPost, etc.
        builder.Services.AddEndpointsApiExplorer();


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

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath); // For XML comments
            c.EnableAnnotations(); // For [SwaggerOperation]
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

        // 1. CORS policy
        app.UseCors("AllowAll");

        // 2. Exception handling (dev/prod)
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
        }

        // 3. Swagger (UI e JSON)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Power Platorm Custom Connectors - Teched 2025 V1");
            c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
        });


        // 4. HTTPS Redirection
        app.UseHttpsRedirection();


        // 5. Routing
        app.UseRouting();

        // 6. Authentication/Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // 7. Custom middlewares (always AFTER Routing/Authentication/Authorization)


        // Middleware 1 for authentication via API Key
        app.UseMiddleware<ApiKeyAuthMiddleware>();

        // Middleware 2 Logging
        app.UseMiddleware<LoggingMiddleware>();

        // Middleware 3 for GitHub signature validation
        app.UseMiddleware<GitHubSignatureValidationMiddleware>();


        // 8. Endpoints
        app.MapControllers();
        app.MapSwagger().RequireAuthorization();

        app.MapGet("/", () => "Hello, TechEd 2025!")
            .WithName("RootGreeting")
            .WithTags("Root")
            .WithOpenApi(op =>
            {
                op.Summary = "Root page.";
                op.Description = "This endpoint returns information on the application root.";
                return op;
            });

        app.MapGet("/api/hello", () =>
            {
                var response = new HelloResponse
                {
                    Message = "Hello from the API!"
                };

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ApiGreeting")
            .WithTags("Greetings")
            .Produces<HelloResponse>(StatusCodes.Status200OK)
            .WithOpenApi(op =>
            {
                op.Summary = "API Authenticated Greeting";
                op.Description = "This endpoint returns a greeting, but requires authentication.";
                return op;
            });

        app.MapGet("/api/hello/{name}", (string name) =>
            {
                var response = new HelloResponse
                {
                    Message = $"Hello, {name} from the API!"
                };

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("SayHello")
            .WithTags("Greetings")
            .Produces<HelloResponse>(StatusCodes.Status200OK)
            .WithOpenApi(op =>
            {
                op.Summary = "Say hello to a person";
                op.Description = "This endpoint returns a custom greeting with the given name, but requires authentication.";
                return op;
            });

        app.Run();
    }
}
