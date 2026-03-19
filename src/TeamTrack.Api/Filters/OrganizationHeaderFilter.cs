using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TeamTrack.Api.Filters
{
    public class OrganizationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Organization-Id",
                In = ParameterLocation.Header,
                Required = false, // make true later if needed
                Description = "Organization Id for multi-tenant context",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}