using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Web.Api.Data.Contracts;

namespace Web.Api.Data.Helpers.Repositories
{
    public interface IRepository<T> : IReadRepository<T>, IDisposable where T : class, IEntity
    {
        T GetById(int id);
        T GetByIdWithException(int id);
        IQueryable<T> GetAll();

        bool Exists(int id);
        bool Exists(Expression<Func<T, bool>> predicate);

        T Create(T entity);
        void Create(IEnumerable<T> entities);

        T Update(T entity);
        void Update(IEnumerable<T> entities);

        void Delete(int id);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}