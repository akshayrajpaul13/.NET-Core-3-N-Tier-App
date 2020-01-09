using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Web.Api.Base.Extensions;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public sealed class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Updates permissions for a role or optionally all users of the given type
        /// </summary>
        /// <param name="roleId">The role to update</param>
        /// <param name="permissions">Pairs of permission type and enabled value</param>
        /// <param name="overwriteUserPermissions">Whether to update all users on this role to match the role permissions</param>
        public void UpdateRolePermissions(int roleId, Dictionary<int, bool> permissions, bool overwriteUserPermissions)
        {
            //if (roleId == Roles.Student.ToInt())
            //    return;

            var existingPermissions = Uow.PermissionQueries.GetAllForRole(roleId);
            var newPermissions = new List<Permission>();

            using (var transaction = new TransactionScope())
            {
                // Update existing permissions if available, otherwise insert new permission entries into the db
                foreach (var permission in permissions)
                {
                    var permissionTypeId = permission.Key;
                    var enabled = permission.Value;

                    var existingPermission = existingPermissions.FirstOrDefault(x => x.PermissionTypeId == permissionTypeId);
                    if (existingPermission != null)
                    {
                        existingPermission.Enabled = enabled;
                    }
                    else
                    {
                        newPermissions.Add(new Permission
                        {
                            PermissionTypeId = permissionTypeId,
                            RoleId = roleId,
                            Enabled = enabled,
                        });
                    }
                }

                Uow.Permissions.Create(newPermissions);

                // Update all users if instructed so
                if (overwriteUserPermissions)
                    ResetAllUserPermissionsOnRole(roleId);

                transaction.Complete();
            }
        }

        /// <summary>
        /// Resets all permissions for all users on the given role, to match back to the defaults of the role
        /// </summary>
        private void ResetAllUserPermissionsOnRole(int roleId)
        {
            var users = Uow.UserQueries.GetAllByRoleId(roleId);
            foreach (var user in users)
            {
                ResetUserPermissionsToRoleDefaults(user.Id);
            }
        }

        /// <summary>
        /// Resets all permissions for a user back to the defaults for the role
        /// </summary>
        public void ResetUserPermissionsToRoleDefaults(int userId)
        {
            var user = Uow.Users.GetByIdWithException(userId);
            //if (user.RoleId == Roles.Student.ToInt())
            //    return;

            var rolePermissions = Uow.PermissionQueries.GetAllForRole(user.RoleId);

            UpdateUserPermissions(userId, rolePermissions.ToDictionary(x => x.PermissionTypeId, x => x.Enabled));
        }

        /// <summary>
        /// Updates permissions for a user
        /// </summary>
        /// <param name="userId">The user to update</param>
        /// <param name="permissions">Pairs of permission type and enabled value</param>
        public void UpdateUserPermissions(int userId, Dictionary<int, bool> permissions)
        {
            var user = Uow.Users.GetByIdWithException(userId);
            //if (user.RoleId == Roles.Student.ToInt())
            //    return;

            var existingPermissions = Uow.PermissionQueries.GetAllForUser(userId);
            var newPermissions = new List<Permission>();

            using (var transaction = new TransactionScope())
            {
                // Update existing permissions if available, otherwise insert new permission entries into the db
                foreach (var permission in permissions)
                {
                    var permissionTypeId = permission.Key;
                    var enabled = permission.Value;

                    var existingPermission = existingPermissions.FirstOrDefault(x => x.PermissionTypeId == permissionTypeId);
                    if (existingPermission != null)
                    {
                        existingPermission.Enabled = enabled;
                    }
                    else
                    {
                        newPermissions.Add(new Permission
                        {
                            PermissionTypeId = permissionTypeId,
                            UserId = userId,
                            Enabled = enabled,
                        });
                    }
                }

                Uow.Permissions.Update(existingPermissions);
                Uow.Permissions.Create(newPermissions);

                transaction.Complete();
            }
        }

        /// <summary>
        /// Creates a permission on all roles and users for the given permission type, with a named list of roles
        /// having the permission enabled, and all other roles having the permission disabled
        /// </summary>
        public void CreatePermissionForAllRolesAndUsers(PermissionTypes permissionType, Roles[] rolesWithPermissionEnabled)
        {
            using (var transaction = new TransactionScope())
            {
                var enabledRoleIds = rolesWithPermissionEnabled.Select(x => x.ToInt()).ToList();
                var permissions = new List<Permission>();

                // Add for roles
                var roles = Uow.Roles.GetAll();
                foreach (var role in roles)
                {
                    //if (role.Id == Roles.Student.ToInt())
                    //    continue;

                    permissions.Add(new Permission
                    {
                        PermissionTypeId = permissionType.ToInt(),
                        RoleId = role.Id,
                        Enabled = enabledRoleIds.Contains(role.Id),
                    });
                }

                // Add for users, based on their roles
                var users = Uow.Users.GetAll();
                foreach (var user in users)
                {
                    permissions.Add(new Permission
                    {
                        PermissionTypeId = permissionType.ToInt(),
                        UserId = user.Id,
                        Enabled = enabledRoleIds.Contains(user.RoleId),
                    });
                }

                // Commit to db, since this can be heavy we need to use an optimised process
                new BatchedEntityCreator<Permission>().Create(permissions);

                transaction.Complete();
            }
        }

        /// <summary>
        /// Deletes all the permission types and associated permissions as indicated
        /// </summary>
        public void DeletePermissionTypes(List<PermissionType> permissionTypes)
        {
            if (permissionTypes.Empty())
                return;

            using (var transaction = new TransactionScope())
            {
                foreach (var permissionType in permissionTypes)
                {
                    var permissions = Uow.PermissionQueries.GetAllForPermissionType(permissionType.Id);
                    Uow.Permissions.Delete(permissions);
                }

                Uow.PermissionTypes.Delete(permissionTypes);

                transaction.Complete();
            }
        }
    }
}
