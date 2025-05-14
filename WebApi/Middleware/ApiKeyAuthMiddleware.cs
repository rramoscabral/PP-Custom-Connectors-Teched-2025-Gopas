
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext;

namespace MyAppDemo.WebAPI.Middleware;

/// <summary>
/// Middleware to authenticate requests using an API key.
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;


    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _next = next;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key Missing");
            return;
        }

        // Resolve WebAPIDbContext within a scope
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WebAPIDbContext>();


        // Check if the API key is valid by querying the database table AuthorizedEmails.
        var apiKey = extractedApiKey.ToString();
        var authorizedUser = await dbContext.AuthorizedEmails
            .FirstOrDefaultAsync(u => u.ApiKey == apiKey);

        if (authorizedUser == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized.");
            return;
        }

        
        // Stores the email in context for later use
        context.Items["UserEmail"] = authorizedUser.Email;


        // Uncomment the following lines if you want to validate the API key against a configuration value
        /*
        var apiKey = _configuration["ApiKey"];
        
        if (apiKey == null || !apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }
        */

        await _next(context);
    }
}