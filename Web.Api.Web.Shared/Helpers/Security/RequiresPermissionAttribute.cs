using Microsoft.AspNetCore.Authorization;
using System;
using Web.Api.Data.Enums;

namespace Web.Api.Web.Shared.Helpers.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class RequiresPermissionAttribute : AuthorizeAttribute
    {
        public RequiresPermissionAttribute(PermissionTypes permission) : base(permission.ToString()) { }
    }
}
