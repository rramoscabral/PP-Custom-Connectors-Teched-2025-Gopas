
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

        builder.Services.AddDbContext<WebAPIDbContext>(options =>
            options.UseSqlServer(connectionString));


        builder.Services.AddScoped<IGitHubService, GitHubService>();

        // HttpClient dependency injection
        builder.Services.AddHttpClient<IWebhookService, WebhookService>();
        builder.Services.AddHttpClient<IPerplexityService, PerplexityService>();

        // OpenAPI document generation
        builder.Services.AddOpenApi();

        // Add Swagger generation
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Power Platorm Custom Connectors - Teched 2025", Version = "v1" });
        });

        var app = builder.Build();


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

        app.UseAuthorization();

        
        app.MapControllers();

        app.MapSwagger().RequireAuthorization();

        app.MapGet("/", () => "Hello, TechEd 2025!");
        app.MapGet("/api/hello", () => "Hello from the API!").RequireAuthorization();
        app.MapGet("/api/hello/{name}", (string name) => $"Hello, {name} from the API!").RequireAuthorization();

        app.Run();
    }
}
