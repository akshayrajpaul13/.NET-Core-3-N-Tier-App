using System.Collections.Generic;
using System.Linq;
using Web.Api.Base.Extensions;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Data.Queries
{
    public class PermissionQueries : BaseQueries
    {
        public PermissionQueries(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Permission> GetAllEnabledForUsers()
        {
            return Uow.Permissions.GetAll().Where(x => x.UserId != null && x.Enabled).ToList();
        }

        public List<Permission> GetAllForRole(int roleId)
        {
            return Uow.Permissions.GetAll(x => x.UserId == null && x.RoleId == roleId).ToList();
        }

        public List<Permission> GetAllForUser(int userId)
        {
            return Uow.Permissions.GetAll(x => x.UserId == userId).ToList();
        }

        public List<PermissionTypes> GetAllEnabledForUser(int userId)
        {
            return Uow.Permissions.GetAll(x => x.UserId == userId && x.Enabled).Select(x => (PermissionTypes)x.PermissionTypeId).ToList();
        }

        public List<Permission> GetAllForPermissionType(int permissionTypeId)
        {
            return Uow.Permissions.GetAll(x => x.PermissionTypeId == permissionTypeId).ToList();
        }

        public List<Permission> GetAllForPermissionType(PermissionTypes permissionType)
        {
            return GetAllForPermissionType(permissionType.ToInt());
        }

        public bool UserHasPermission(int userId, PermissionTypes permissionType)
        {
            return Uow.Permissions.GetAll(x => x.UserId == userId && x.PermissionTypeId == (int)permissionType && x.Enabled).Any();
        }

        public bool UserHasAnyOfPermissions(int userId, params PermissionTypes[] permissionTypes)
        {
            var permissionTypeIds = permissionTypes.Select(x => (int)x).ToList();
            return Uow.Permissions.GetAll(x => x.UserId == userId
                                               && permissionTypeIds.Contains(x.PermissionTypeId)
                                               && x.Enabled).Any();
        }
    }
}
