using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Paging;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Web.Shared.Helpers.Security;
using Web.Api.WebApi.Models.User;

namespace Web.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : SecuredController<UserController>
    {
        private readonly UnitOfWork<AppDbContext> _unitOfWork;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork) : base(logger)
        {
            _unitOfWork = unitOfWork as UnitOfWork<AppDbContext>;
        }

        [HttpGet]
        [RequiresPermission(PermissionTypes.UsersView)]
        [AllowAnonymous]
        public ActionResult<IPaginate<UserViewModel>> Index(int? userId)
        {
            var model = UserViewModel.GetAll(_unitOfWork, userId, null);
            return Ok(model);
        }
    }
}