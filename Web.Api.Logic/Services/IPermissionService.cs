using System.Collections.Generic;
using Web.Api.Data.Enums;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public interface IPermissionService
    {
        void UpdateRolePermissions(int roleId, Dictionary<int, bool> permissions, bool overwriteUserPermissions);
        void ResetUserPermissionsToRoleDefaults(int userId);
        void UpdateUserPermissions(int userId, Dictionary<int, bool> permissions);
        void CreatePermissionForAllRolesAndUsers(PermissionTypes permissionType, Roles[] rolesWithPermissionEnabled);
        void DeletePermissionTypes(List<PermissionType> permissionTypes);
    }
}