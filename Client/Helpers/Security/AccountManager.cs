using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Api.Base.Extensions;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Logic.Models.Account;
using Web.Api.Logic.Services;
using Web.Api.Web.Shared.Helpers.Security;

namespace Web.Api.Client.Helpers.Security
{
    public class AccountManager : IAccountManager
    {
        private readonly UnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountManager(IUnitOfWork unitOfWork, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork as UnitOfWork<AppDbContext>;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResult> LogIn(ILoginModel model)
        {
            var user = _unitOfWork.UserQueries.GetByEmailWithPermissions(model.Email);
            var loginResult = _userService.Login(user, model.Password, LoginTypes.Default.ToInt());

            if (!loginResult.Successful)
                return loginResult;

            // Identify login by session guid rather than email, because session guid tells us everything (email, tenant, session) and can't be fabricated
            var permissions = string.Join(',', user.Permissions.Select(x => x.PermissionTypeId));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResult.SessionGuid),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
                new Claim(CustomClaimTypes.TenantId, user.TenantId.ToString()),
                new Claim(CustomClaimTypes.Permissions, permissions),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return loginResult;
        }

        public async Task LogOut()
        {
            var currentGuid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _userService.Logout(currentGuid);
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
