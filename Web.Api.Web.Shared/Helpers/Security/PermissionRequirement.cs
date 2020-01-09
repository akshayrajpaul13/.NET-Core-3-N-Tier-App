using Microsoft.AspNetCore.Authorization;
using Web.Api.Data.Enums;

namespace Web.Api.Web.Shared.Helpers.Security
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionTypes Permission { get; }

        public PermissionRequirement(PermissionTypes permission)
        {
            Permission = permission;
        }
    }
}
