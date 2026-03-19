using System.Diagnostics;

namespace TeamTrack.Api.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate _next, ILogger<RequestLoggingMiddleware> _logger)
    {
        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var correlationId = context.Items["X-Correlation-Id"]?.ToString();

            _logger.LogInformation(
                "Request {Method} {Path} | CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Response {StatusCode} in {Elapsed} ms | CorrelationId: {CorrelationId}",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
    }
}