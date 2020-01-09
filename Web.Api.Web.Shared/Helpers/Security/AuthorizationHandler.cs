using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Base.Extensions;
using Web.Api.Data.Enums;

namespace Web.Api.Web.Shared.Helpers.Security
{
    public class AuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public AuthorizationHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var permissionsClaim = context.User?.FindFirst(CustomClaimTypes.Permissions);
            // If user does not have the claim, get out of here
            if (permissionsClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var permissions = permissionsClaim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            if (permissions.Contains(requirement.Permission.ToInt()))
                context.Succeed(requirement);
            else
                context.Fail();


            return Task.CompletedTask;
        }
    }
}
