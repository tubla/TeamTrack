using System.Net;
using System.Text.Json;
using TeamTrack.Api.Common;
using TeamTrack.Api.Exceptions;

namespace TeamTrack.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Failure(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = ApiResponse<string>.Failure("Something went wrong");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}