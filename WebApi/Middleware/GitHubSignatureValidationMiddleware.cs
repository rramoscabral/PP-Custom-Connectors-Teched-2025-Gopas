using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyAppDemo.DataLayer.DBContext;

namespace MyAppDemo.WebAPI.Middleware;

public class GitHubSignatureValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GitHubSignatureValidationMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public GitHubSignatureValidationMiddleware(RequestDelegate next, ILogger<GitHubSignatureValidationMiddleware> logger, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("GitHubSignatureValidationMiddleware middleware run");

            if (context.Request.Path.StartsWithSegments("/api/github/issue-webhook", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.EnableBuffering();

                // Read the original body exactly as it arrived
                string body;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                }
                context.Request.Body.Position = 0;

                // Only for debug purposes this will contain credentials 
                //_logger.LogInformation("GitHubSignatureValidationMiddleware Body: {Body}", body);

                if (!context.Request.Headers.TryGetValue("X-Hub-Signature-256", out var signatureHeader))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing signature.");
                    return;
                }


                // Use the original body to calculate the HMAC
                // (don't extract the inner JSON if it is x-www-form-urlencoded!)
                string signature = signatureHeader.ToString();


                // Detect the content type and extract the JSON correctly
                string jsonBody = null;
                if (context.Request.ContentType != null &&
                    context.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    var parsed = System.Web.HttpUtility.ParseQueryString(body);
                    jsonBody = parsed["payload"];
                }
                else if (context.Request.ContentType != null &&
                    context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    jsonBody = body;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    await context.Response.WriteAsync("Unsupported Content-Type");
                    return;
                }


                if (string.IsNullOrWhiteSpace(jsonBody))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Payload is missing.");
                    return;
                }


                // Extract repository name and owner from JSON
                var jsonDoc = JsonDocument.Parse(jsonBody);
                var repoName = jsonDoc.RootElement.GetProperty("repository").GetProperty("name").GetString();
                var ownerLogin = jsonDoc.RootElement.GetProperty("repository").GetProperty("owner").GetProperty("login").GetString();

                // Access the database context
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<WebAPIDbContext>();

                var repo = await dbContext.GitHubRepositories
                    .FirstOrDefaultAsync(r => r.RepositoryName == repoName && r.OwnerName == ownerLogin);

                if (repo == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Repository not found.");
                    return;
                }

                // Calculate HMAC using body (original)
                var expectedSignature = $"sha256={ComputeHmacSha256(body, repo.WebhookSecret)}";

                // Compare the HMAC signatures
                if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature)))
                {
                    _logger.LogWarning("Invalid signature even though it looks the same.");
                    _logger.LogInformation("Signature header: {Header}", signature);
                    _logger.LogInformation("Expected signature: {Expected}", expectedSignature);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid signature.");
                    return;
                }
            }
            await _next(context); // Only call the next one if there was no error
        }
        catch (Exception ex)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error.");
                return;
            }
            _logger.LogError(ex, "An error occurred in GitHubSignatureValidationMiddleware");
        }
    }

    private static string ComputeHmacSha256(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
