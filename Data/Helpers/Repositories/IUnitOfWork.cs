using Microsoft.EntityFrameworkCore;
using System;
using Web.Api.Data.Contracts;

namespace Web.Api.Data.Helpers.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class, IEntity;
        IRepositoryReadOnly<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : class, IEntity;

        int SaveChanges();
        void Commit();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
