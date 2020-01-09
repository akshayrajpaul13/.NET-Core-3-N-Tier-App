using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Web.Api.Client.Models;
using Web.Api.Data.Enums;
using Web.Api.Web.Shared.Helpers.Keys;
using Web.Api.Web.Shared.Helpers.Security;

namespace Web.Api.Client.Controllers
{
    public class HomeController : SecuredController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger) : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [RequiresPermission(PermissionTypes.Map)]
        public IActionResult Map()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void ToggleSidebar()
        {
            // 0/1 used to limit session memory used
            var currentVal = HttpContext.Session.GetString(SessionKeys.IsSidebarCollapsed) ?? "0";
            currentVal = currentVal == "0" ? "1" : "0";

            HttpContext.Session.SetString(SessionKeys.IsSidebarCollapsed, currentVal);
        }
    }
}