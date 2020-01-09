using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Web.Api.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SecuredController<T> : BaseController<T>
    {
        public SecuredController(ILogger<T> logger) : base(logger)
        {
        }
    }
}