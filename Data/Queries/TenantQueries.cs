using System.Collections.Generic;
using System.Linq;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Data.Queries
{
    public class TenantQueries : BaseQueries
    {
        public TenantQueries(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Tenant> GetAll()
        {
            return Uow.Tenants.GetAll().ToList();
        }
    }
}
