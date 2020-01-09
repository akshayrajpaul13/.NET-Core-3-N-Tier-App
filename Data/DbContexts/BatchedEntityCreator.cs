using System.Collections.Generic;
using System.Linq;
using Web.Api.Data.Contracts;
using Web.Api.Data.Helpers.Repositories;

namespace Web.Api.Data.DbContexts
{
    /// <summary>
    /// Writes new entities to the db in batches to improve performance
    /// </summary>
    public class BatchedEntityCreator<T> where T : class
    {
        public void Create(List<T> entities)
        {
            // Keep batch size small because repo pattern includes transaction and audit logging
            var batchSize = 20;
            var count = 0;

            while (count < entities.Count)
            {
                var batch = entities.Skip(count).Take(batchSize).ToList();
                CreateBatch(batch);

                count += batchSize;
            }
        }

        private void CreateBatch(List<T> batch)
        {
            // Re-instantiate the repo, because dbcontext builds up a cache that we need to clear
            //var repo = new UnitOfWork<AppDbContext>(null);

            // There is a transaction and audit logging inside here
            //repo.GetStandardRepo<T>().Create(batch);
        }
    }
}
