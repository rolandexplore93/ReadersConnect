using Microsoft.EntityFrameworkCore;
using ReadersConnect.Domain.Models;
using ReadersConnect.Application.BaseInterfaces.Abstractions;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Infrastructure.BaseRepository.Implementations;
using ReadersConnect.Infrastructure.Persistence;

namespace ReadersConnect.Infrastructure.UnitOfWork
{
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork
    where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories = new();

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(Context);
            return (IRepositoryBase<TEntity>)_repositories[type];
        }

        public TContext Context { get; }

        public int SaveChanges()
        {
            var entries = Context.ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntries in entries)
            {
                ((BaseEntity)entityEntries.Entity).ModifiedAt = DateTime.UtcNow;
                if (entityEntries.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntries.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return Context.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public void DetachEntity<T>(T entity) where T : class
        {
            if (entity != null)
            {
                Context.Entry(entity).State = EntityState.Detached;
            }
        }
    }
}