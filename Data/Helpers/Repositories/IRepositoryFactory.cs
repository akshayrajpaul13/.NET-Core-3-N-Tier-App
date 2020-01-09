using System;
using System.Collections.Generic;
using System.Text;
using Web.Api.Data.Contracts;

namespace Web.Api.Data.Helpers.Repositories
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity;
        IRepositoryAsync<T> GetRepositoryAsync<T>() where T : class, IEntity;
        IRepositoryReadOnly<T> GetReadOnlyRepository<T>() where T : class, IEntity;
    }
}
