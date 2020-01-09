using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Web.Api.Base.Extensions;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Attributes;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public sealed class PermissionUpdaterService : BaseService, IPermissionUpdaterService
    {
        private readonly IPermissionService _permissionService;

        public PermissionUpdaterService(IUnitOfWork unitOfWork, IPermissionService permissionService) : base(unitOfWork)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Checks for new permission types, adds them automatically to the db and sets the default
        /// permissions for roles and users on this new permission type
        /// </summary>
        public void AddNewPermissionTypes()
        {
            // Get all permission types in the enum and all in the db
            var permissionTypesInDb = Uow.PermissionTypeQueries.GetAll();
            var permissionTypesInCode = EnumExtensions.GetValues<PermissionTypes>();

            // Check for missing permission types in the db
            foreach (var permissionType in permissionTypesInCode.Where(x => x != PermissionTypes.Undefined))
            {
                // Check if the permission type is in the db and create it if not
                var permissionTypeInDb = permissionTypesInDb.FirstOrDefault(x => x.Id == permissionType.ToInt());
                if (permissionTypeInDb == null)
                {
                    CreatePermissionType(permissionType);
                }
            }

            // Find permission types in the db that have been deleted in code and delete them in the db
            var toDelete = new List<PermissionType>(); //can't delete inside foreach
            foreach (var permissionTypeInDb in permissionTypesInDb)
            {
                var permissionTypesInCodeIds = permissionTypesInCode.Select(x => x.ToInt()).ToList();
                if (!permissionTypesInCodeIds.Any(x => x == permissionTypeInDb.Id))
                {
                    toDelete.Add(permissionTypeInDb);
                }
            }
            _permissionService.DeletePermissionTypes(toDelete);
        }

        /// <summary>
        /// Creates a permission type and populates permissions for all roles and users
        /// </summary>
        private void CreatePermissionType(PermissionTypes permissionType)
        {
            var permissionTypeInDb = new PermissionType
            {
                Id = permissionType.ToInt(),
                Name = GetName(permissionType),
                Description = GetDescription(permissionType),
            };

            Uow.PermissionTypes.Create(permissionTypeInDb);

            // Ensure the permission type is written with the expected id
            if (permissionTypeInDb.Id != permissionType.ToInt())
                throw new Exception($"Permission type {permissionType} was auto-created with id {permissionTypeInDb.Id} but should have been id {permissionType.ToInt()}. This will need to be manually repaired.");

            CreateRoleAndUserPermissions(permissionType);
        }

        private void CreateRoleAndUserPermissions(PermissionTypes permissionType)
        {
            var defaultRoles = GetDefaultRoles(permissionType);

            _permissionService.CreatePermissionForAllRolesAndUsers(permissionType, defaultRoles);
        }

        private string GetName(PermissionTypes permissionType)
        {
            var memberInfo = typeof(PermissionTypes).GetMember(permissionType.ToString()).FirstOrDefault();
            var attribute = (PermissionNameAttribute)memberInfo.GetCustomAttributes(typeof(PermissionNameAttribute), false).FirstOrDefault();
            if (attribute == null)
                throw new Exception("PermissionName attribute is missing for the permission type " + permissionType);

            return attribute.Name;
        }

        private string GetDescription(PermissionTypes permissionType)
        {
            var memberInfo = typeof(PermissionTypes).GetMember(permissionType.ToString()).FirstOrDefault();
            var attribute = (DescriptionAttribute)memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            if (attribute == null)
                throw new Exception("Description attribute is missing for the permission type " + permissionType);

            return attribute.Description;
        }

        private Roles[] GetDefaultRoles(PermissionTypes permissionType)
        {
            var memberInfo = typeof(PermissionTypes).GetMember(permissionType.ToString()).FirstOrDefault();
            var attribute = (DefaultRolesAttribute)memberInfo.GetCustomAttributes(typeof(DefaultRolesAttribute), false).FirstOrDefault();
            if (attribute == null)
                throw new Exception("DefaultRoles attribute is missing for the permission type " + permissionType);

            return attribute.DefaultRoles;
        }
    }
}
