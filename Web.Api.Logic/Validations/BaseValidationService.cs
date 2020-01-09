using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Repositories;

namespace Web.Api.Logic.Validations
{
    public class BaseValidationService : IValidationService
    {
        protected UnitOfWork<AppDbContext> UnitOfWork { get; }

        public BaseValidationService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork as UnitOfWork<AppDbContext>;
        }
    }
}
