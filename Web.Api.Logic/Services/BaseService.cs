using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Repositories;

namespace Web.Api.Logic.Services
{
    public class BaseService : IService
    {
        protected UnitOfWork<AppDbContext> Uow { get; }

        public BaseService(IUnitOfWork unitOfWork)
        {
            Uow = unitOfWork as UnitOfWork<AppDbContext>;
        }
    }
}
