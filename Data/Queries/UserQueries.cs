using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Paging;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Data.Queries
{
    public class UserQueries : BaseQueries
    {
        public UserQueries(IUnitOfWork repo) : base(repo)
        {
        }

        public User Get(int userId, bool throwExceptionIfNotFound = false)
        {
            var user = Uow.Users.GetById(userId);
            if (throwExceptionIfNotFound && user == null)
                throw new Exception("User not found for id: " + userId);

            return user;
        }

        public User GetByEmail(string email, bool throwExceptionIfNotFound = false)
        {
            var user = Uow.Users.FirstOrDefault(x => x.Email == email);
            if (throwExceptionIfNotFound && user == null)
                throw new Exception("User not found with email: " + email);

            return user;
        }

        public User GetByEmailWithPermissions(string email)
        {
            var user = Uow.Users.FirstOrDefault(
                x => x.Email == email,
                include: users => users.Include(x => x.Permissions)
            );
            if (user == null)
                throw new Exception("User not found with email: " + email);

            return user;
        }

        public bool DoesVerificationCodeExist(string verificationCode)
        {
            return Uow.Users.GetAll(u => u.VerificationCode == verificationCode).Any();
        }

        /// <summary>
        /// Gets all users that have a given permission
        /// </summary>
        public List<User> GetAllWithPermission(PermissionTypes permissionType)
        {
            return (from u in Uow.Users.GetAll()
                    join p in Uow.Permissions.GetAll() on u.Id equals p.UserId
                    where p.PermissionTypeId == (int)permissionType && p.Enabled
                    select u)
                .ToList();
        }

        public List<User> GetAllByRoleId(int roleId, int? tenantId = null)
        {
            return Uow.Users.GetAll(
                    x => x.RoleId == roleId && (tenantId == null || x.TenantId == null || x.TenantId == tenantId),
                    users => users.OrderBy(x => x.Email)
                ).ToList();
        }

        public List<User> GetAllByRole(Roles role, int? tenantId = null)
        {
            return Uow.Users.GetAll(
                x => x.RoleId == (int)role && (tenantId == null || x.TenantId == null || x.TenantId == tenantId),
                users => users.OrderBy(x => x.Email)
            ).ToList();
        }
    }
}
