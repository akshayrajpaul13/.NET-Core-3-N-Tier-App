using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Repositories;

namespace Web.Api.Data.Queries
{
    public abstract class BaseQueries
    {
        protected UnitOfWork<AppDbContext> Uow { get; }

        protected BaseQueries(IUnitOfWork unitOfWork)
        {
            Uow = unitOfWork as UnitOfWork<AppDbContext>;
        }
    }
}