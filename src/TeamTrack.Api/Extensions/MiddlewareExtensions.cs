using TeamTrack.Api.Middleware;
using TeamTrack.Api.Hubs;

namespace TeamTrack.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app, IWebHostEnvironment env)
    {
        // Global middleware
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRateLimiter();
        app.UseForwardedHeaders();
        
        // Swagger
        var swaggerEnabled = env.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled");
        if (swaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamTrack API v1");
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
                options.DefaultModelsExpandDepth(1);
                options.DefaultModelExpandDepth(1);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });
        }

        // Security

        if (app.Environment.IsDevelopment())
        {
            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }
        else
        {
            app.UseCors("AllowConfiguredOrigins");
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Tenant context after authentication
        app.UseMiddleware<TenantContextMiddleware>();
        
        app.UseStaticFiles();

        // Endpoints
        app.MapHub<TeamTrackHub>("/hubs/teamtrack");
        app.MapControllers();
        
        return app;
    }
}