using System.Collections.Generic;
using System.Linq;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Data.Queries
{
    public class PermissionTypeQueries : BaseQueries
    {
        public PermissionTypeQueries(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<PermissionType> GetAll()
        {
            return Uow.PermissionTypes.GetAll().ToList();
        }
    }
}
