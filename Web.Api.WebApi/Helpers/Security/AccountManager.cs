using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Api.Base.Extensions;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Logic.Models.Account;
using Web.Api.Logic.Services;
using Web.Api.Web.Shared.Helpers.Security;
using Web.Api.WebApi.Helpers.Exceptions;

namespace Web.Api.WebApi.Helpers.Security
{
    public class AccountManager : IAccountManager
    {
        private readonly UnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;

        public AccountManager(IUnitOfWork unitOfWork, IUserService userService, IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork as UnitOfWork<AppDbContext>;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
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

            // authentication successful so generate jwt token
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            loginResult.Token = tokenHandler.WriteToken(token);

            //await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return loginResult;
        }

        public async Task LogOut()
        {
            var currentGuid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _userService.Logout(currentGuid);
            // await _httpContextAccessor.HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        }
    }
}
