using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Paging;
using Web.Api.Data.Helpers.Repositories;

namespace Web.Api.WebApi.Models.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string Tenant { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLocked { get; set; }
        public string EmployeeNumber { get; set; }

        public static IPaginate<UserViewModel> GetAll(UnitOfWork<AppDbContext> uow, int? userId, int? roleId)
        {
            var users = uow.Users.GetList(
                u => new UserViewModel
                {
                    Id = u.Id,
                    TenantId = u.TenantId,
                    Tenant = u.Tenant == null ? "" : u.Tenant.Name,
                    RoleId = u.RoleId,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    //FirstName = u.UserProfile.FirstName,
                    //LastName = u.UserProfile.LastName,
                    //EmployeeNumber = u.UserProfile.EmployeeNumber,
                    IsLocked = u.LockoutEndDateUtc != null && u.LockoutEndDateUtc > DateTime.UtcNow,
                },
                u => (u.Id == userId || userId == null) && (u.RoleId == roleId || roleId == null),
                u => u.OrderBy(x => x.Email),
                u => u.Include(x => x.Tenant),
                0,
                10);

            return users;
        }
    }
}
