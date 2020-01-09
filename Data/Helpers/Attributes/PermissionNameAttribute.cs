using System;

namespace Web.Api.Data.Helpers.Attributes
{
    /// <summary>
    /// Defines the visible name of a permission type (rather than the pascalised code name). The name should be
    /// human-readable.
    /// </summary>
    public sealed class PermissionNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
    }
}