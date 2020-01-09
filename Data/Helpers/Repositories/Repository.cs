using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Web.Api.Base.Extensions;
using Web.Api.Data.Contracts;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Auditing;

namespace Web.Api.Data.Helpers.Repositories
{
    /// <summary>
    /// Provides the standard mechanism for creating, updating and deleting entities.
    /// Unless there is a strong reason, all such operations should go through this class,
    /// as it also includes automated auditing.
    /// The main hassle with this class is that there may be times when you need more precise
    /// control over the time when context.SaveChanges (repo.Commit) happens, particularly if you want to
    /// combine db calls across several object types.  If you do bypass this class, you'll need
    /// to call AuditLogger directly to get audit logging done.
    /// </summary>
    public class Repository<T> : BaseRepository<T>, IRepository<T> where T : class, IEntity
    {
        private readonly int? _userId;
        
        public Repository(AppDbContext context, int? userId) : base(context)
        {
            _userId = userId.IsSet() ? userId : null;
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Gets by id and throws an exception if not found
        /// </summary>
        public T GetByIdWithException(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
                throw new Exception($"{typeof(T).Name} not found for id {id}");

            return entity;
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public bool Exists(int id)
        {
            return _dbSet.Find(id) != null;
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public void Create(IEnumerable<T> entities)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var entity in entities)
                {
                    var dbEntityEntry = _dbContext.Entry(entity);

                    if (dbEntityEntry.State != EntityState.Detached)
                    {
                        dbEntityEntry.State = EntityState.Added;
                    }
                }

                _dbSet.AddRange(entities);

                var auditLogService = new AuditLogger(_dbContext, _userId);
                auditLogService.AuditCreatedBefore(entities.ToList<IEntity>());

                _dbContext.SaveChanges();

                auditLogService.AuditCreatedAfter(entities.ToList<IEntity>());

                transaction.Complete();
            }
        }

        public T Create(T entity)
        {
            using (var transaction = new TransactionScope())
            {
                var dbEntityEntry = _dbContext.Entry(entity);

                if (dbEntityEntry.State != EntityState.Detached)
                {
                    dbEntityEntry.State = EntityState.Added;
                }
                else
                {
                    _dbSet.Add(entity);
                }

                var auditLogService = new AuditLogger(_dbContext, _userId);
                auditLogService.AuditCreatedBefore(new List<IEntity> { entity });

                _dbContext.SaveChanges();

                auditLogService.AuditCreatedAfter(new List<IEntity> { entity });

                transaction.Complete();

                return entity;
            }
        }

        public T Update(T entity)
        {
            using (var transaction = new TransactionScope())
            {
                new AuditLogger(_dbContext, _userId).AuditUpdated(new List<IEntity> { entity });

                var dbEntityEntry = _dbContext.Entry(entity);

                if (dbEntityEntry.State != EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }

                dbEntityEntry.State = EntityState.Modified;

                _dbContext.SaveChanges();

                transaction.Complete();

                return entity;
            }
        }

        public void Update(IEnumerable<T> entities)
        {
            using (var transaction = new TransactionScope())
            {
                new AuditLogger(_dbContext, _userId).AuditUpdated(entities.ToList<IEntity>());

                foreach (var entity in entities)
                {
                    var dbEntityEntry = _dbContext.Entry(entity);

                    if (dbEntityEntry.State != EntityState.Detached)
                    {
                        _dbSet.Attach(entity);
                    }

                    dbEntityEntry.State = EntityState.Modified;
                }

                _dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void Delete(int id)
        {
            using (var transaction = new TransactionScope())
            {
                T entity = GetById(id);
                if (entity == null) return;

                Delete(entity);

                transaction.Complete();
            }
        }

        public void Delete(T entity)
        {
            using (var transaction = new TransactionScope())
            {
                var dbEntityEntry = _dbContext.Entry(entity);

                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    _dbSet.Attach(entity);
                    _dbSet.Remove(entity);
                }

                new AuditLogger(_dbContext, _userId).AuditDeleted(new List<IEntity> { entity });

                _dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void Delete(IEnumerable<T> entities)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var entity in entities)
                {
                    var dbEntityEntry = _dbContext.Entry(entity);

                    if (dbEntityEntry.State != EntityState.Deleted)
                    {
                        dbEntityEntry.State = EntityState.Deleted;
                    }
                    else
                    {
                        _dbSet.Attach(entity);
                        _dbSet.Remove(entity);
                    }
                }

                new AuditLogger(_dbContext, _userId).AuditDeleted(entities.ToList<IEntity>());

                _dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}