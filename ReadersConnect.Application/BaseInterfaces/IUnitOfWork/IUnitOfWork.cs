using ReadersConnect.Application.BaseInterfaces.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.BaseInterfaces.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void DetachEntity<T>(T entity) where T : class;
        IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}
