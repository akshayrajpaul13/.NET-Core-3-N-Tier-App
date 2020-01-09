using System.ComponentModel;
using Web.Api.Data.Helpers.Attributes;

namespace Web.Api.Data.Enums
{
    public enum PermissionTypes
    {
        [PermissionName(Name = "Undefined", GroupName = "Undefined")]
        [Description("Undefined")]
        [DefaultRoles(Roles.Undefined)]
        Undefined = 0,

        [PermissionName(Name = "Map", GroupName = "Map")]
        [Description("View Map")]
        [DefaultRoles(Roles.SystemAdmin)]
        Map = 1,

        [PermissionName(Name = "View Users", GroupName = "Users")]
        [Description("View Users")]
        [DefaultRoles(Roles.SystemAdmin)]
        UsersView = 2,
    }
}
