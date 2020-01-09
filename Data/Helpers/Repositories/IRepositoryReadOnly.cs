using Web.Api.Data.Contracts;

namespace Web.Api.Data.Helpers.Repositories
{
    public interface IRepositoryReadOnly<T> : IReadRepository<T> where T : class, IEntity
    {

    }
}
