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
        _logger.LogInformation("Response : {statusCode}", context.Response.StatusCode);
    }
}