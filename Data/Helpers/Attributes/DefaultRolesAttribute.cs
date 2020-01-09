using System;
using Web.Api.Data.Enums;

namespace Web.Api.Data.Helpers.Attributes
{
    /// <summary>
    /// Defines the default roles that are enabled for a permission type
    /// </summary>
    public class DefaultRolesAttribute : Attribute
    {
        public Roles[] DefaultRoles { get; set; }

        public DefaultRolesAttribute(params Roles[] defaultRoles)
        {
            DefaultRoles = defaultRoles;
        }
    }
}