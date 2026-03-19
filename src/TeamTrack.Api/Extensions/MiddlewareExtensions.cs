using TeamTrack.Api.Middleware;

namespace TeamTrack.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseApplicationMiddleware(this WebApplication app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRateLimiter();
            app.UseForwardedHeaders();
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
            ////if (env.IsDevelopment())
            ////{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(options =>
            //    {
            //        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamTrack API v1");
            //        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            //    });
            ////}

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}