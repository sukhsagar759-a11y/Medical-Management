namespace CompX.Api.Common;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ProblemEnvelope(
                "https://httpstatuses.com/500",
                "Internal Server Error",
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                context.TraceIdentifier);

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
