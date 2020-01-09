using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Web.Api.Logic.Services
{
    public sealed class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentTenant()
        {
            throw new NotImplementedException();
        }
    }
}
