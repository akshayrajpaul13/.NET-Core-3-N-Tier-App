using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Web.Api.Base.Extensions;
using Web.Api.Logic.Models.Account;
using Web.Api.Web.Shared.Helpers.Security;

namespace Web.Api.Client.Controllers
{
    public class AccountController : SecuredController<AccountController>
    {
        private readonly IAccountManager _accountManager;

        public AccountController(ILogger<AccountController> logger, IAccountManager accountManager) : base(logger)
        {
            _accountManager = accountManager;
        }

        [AllowAnonymous]
        public virtual IActionResult AccessDenied(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                returnUrl = "";
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (returnUrl == "/Account/Logout" && Url.IsLocalUrl(returnUrl))
                returnUrl = "";
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                    ThrowModelStateErrors();

                var result = await _accountManager.LogIn(model);

                return returnUrl.IsSet() ? RedirectToLocal(returnUrl) : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                SetViewError(ex);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            await _accountManager.LogOut();
            return RedirectToAction("Login", "Account");
        }
    }
}