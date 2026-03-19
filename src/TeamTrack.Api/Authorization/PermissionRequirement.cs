using Microsoft.AspNetCore.Authorization;

namespace TeamTrack.Api.Authorization
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
