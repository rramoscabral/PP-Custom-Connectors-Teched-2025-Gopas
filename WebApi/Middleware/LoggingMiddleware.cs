using System.Text;

namespace MyAppDemo.WebAPI.Middleware;


/// <summary>
/// Middleware to log incoming requests and outgoing responses.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request : {method} {url}", context.Request.Method, context.Request.Path);
        await _next(context);


        // Warning: Only do this in a development environment because this will expose sensitive data.
        // Log the request body (only if it is application/json)
        if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            _logger.LogInformation("Request Body: {body}", body);
        }

        await _next(context);

        _logger.LogInformation("Response : {statusCode}", context.Response.StatusCode);
    }
}