using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Web.Api.Base.Extensions;
using Web.Api.Data.Contracts;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Models;
using Web.Api.Data.Queries;

namespace Web.Api.Data.Helpers.Repositories
{
    public sealed class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext> where TContext : AppDbContext, IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Dictionary<Type, object> _repositories;

        public TContext Context { get; }
        public int? UserId => _httpContextAccessor?.HttpContext?.User.FindFirst(CustomClaimTypes.UserId)?.Value.ToInt();

        #region Generic Model Repo's
        public IRepository<User> Users => GetRepository<User>();
        public IRepository<Session> Sessions => GetRepository<Session>();
        public IRepository<Permission> Permissions => GetRepository<Permission>();
        public IRepository<PermissionType> PermissionTypes => GetRepository<PermissionType>();
        public IRepository<Role> Roles => GetRepository<Role>();
        public IRepository<Tenant> Tenants => GetRepository<Tenant>();
        #endregion

        #region Queries
        public UserQueries UserQueries => new UserQueries(this);
        public SessionQueries SessionQueries => new SessionQueries(this);
        public PermissionQueries PermissionQueries => new PermissionQueries(this);
        public PermissionTypeQueries PermissionTypeQueries => new PermissionTypeQueries(this);
        #endregion

        public UnitOfWork(TContext context, IHttpContextAccessor httpContextAccessor)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(Context, UserId);
            return (IRepository<TEntity>)_repositories[type];
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class, IEntity
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositoryAsync<TEntity>(Context);
            return (IRepositoryAsync<TEntity>)_repositories[type];
        }

        public IRepositoryReadOnly<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : class, IEntity
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositoryReadOnly<TEntity>(Context);
            return (IRepositoryReadOnly<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
