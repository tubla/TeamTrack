using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using System.Security.Claims;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Middleware;

public class TenantContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    
    // Routes that don't require org context
    private static readonly HashSet<string> OrgAgnosticPaths = 
    [
        "/api/v1/auth/login",
        "/api/v1/auth/register",
        "/api/v1/auth/switch-organization",
        "/api/v1/auth/me",              // Profile can work without org
        "/api/v1/auth/refresh-token",   // Refresh can work without org
        "/api/v1/organizations/my",     // Get user's orgs
        "/api/v1/organizations",        // Create org (POST only)
        "/api/v1/org-access/request",        // Create org (POST only)
        "/swagger",
        "/health"
    ];

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Skip org validation for org-agnostic routes
        if (OrgAgnosticPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        // For protected routes, require organization context
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var orgIdClaim = context.User.FindFirst("organizationId")?.Value;
            
            // Also check header as fallback (useful for org switching scenarios)
            var orgIdHeader = context.Request.Headers["X-Organization-Id"].FirstOrDefault();
            
            var orgIdString = !string.IsNullOrEmpty(orgIdClaim) && orgIdClaim != Guid.Empty.ToString()
                ? orgIdClaim
                : orgIdHeader;

            if (string.IsNullOrEmpty(orgIdString) || !Guid.TryParse(orgIdString, out var orgId))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    error = "Organization context required",
                    code = "ORG_CONTEXT_MISSING",
                    message = "Please select an organization to continue"
                });
                return;
            }

            // Validate user is member of this organization
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var isMember = await db.OrganizationUsers
                    .AnyAsync(ou => ou.UserId == userId && ou.OrganizationId == orgId);

                if (!isMember)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        error = "Access denied",
                        code = "ORG_ACCESS_DENIED",
                        message = "You are not a member of this organization"
                    });
                    return;
                }
            }

            // Store in HttpContext for IRequestContext to access
            context.Items["OrganizationId"] = orgId;
        }

        await _next(context);
    }
}