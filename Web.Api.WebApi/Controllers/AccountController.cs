using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Api.Logic.Models.Account;
using Web.Api.Web.Shared.Helpers.Security;

namespace Web.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : SecuredController<AccountController>
    {
        private readonly IAccountManager _accountManager;

        public AccountController(ILogger<AccountController> logger, IAccountManager accountManager) : base(logger)
        {
            _accountManager = accountManager;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
               ThrowModelStateErrors();

            var result = await _accountManager.LogIn(model);
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountManager.LogOut();
            return Ok();
        }
    }
}
