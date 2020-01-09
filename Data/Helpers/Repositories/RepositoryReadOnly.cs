using Web.Api.Data.Contracts;
using Web.Api.Data.DbContexts;

namespace Web.Api.Data.Helpers.Repositories
{
    public class RepositoryReadOnly<T> : BaseRepository<T>, IRepositoryReadOnly<T> where T : class, IEntity
    {
        public RepositoryReadOnly(AppDbContext context) : base(context)
        {
        }
    }
}
