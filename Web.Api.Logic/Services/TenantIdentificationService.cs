using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Web.Api.Logic.Services
{
    public sealed class TenantIdentificationService : ITenantIdentificationService
    {
        private readonly ITenantService _tenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public TenantIdentificationService(ITenantService tenantService, IHttpContextAccessor httpContextAccessor)
        {
            _tenantService = tenantService;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentTenant()
        {
            //var currentUser = _httpContextAccessor.HttpContext.User.FindFirst();
            //if(_httpContextAccessor.)
            return "";
        }
    }
}
