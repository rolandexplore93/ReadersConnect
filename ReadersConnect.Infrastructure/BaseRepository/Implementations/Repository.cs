using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ReadersConnect.Domain.Models;
using System.Linq.Expressions;
using Mapster;
using ReadersConnect.Application.BaseInterfaces.Abstractions;

namespace ReadersConnect.Infrastructure.BaseRepository.Implementations
{
    public class Repository<T> : IRepositoryBase<T> where T : class
    {
        private DbContext DbContext { get; set; }
        internal DbSet<T> _dbSet;
        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<T>();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression)
        {
            return await GetAll(expression).FirstOrDefaultAsync();
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        #region AddEntity
        public bool Any()
        {
            return DbContext.Set<T>().Any();
        }

        #endregion

        public void Dispose()
        {
            DbContext?.Dispose();
        }

        public long Count(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().LongCount(expression);
        }
        public Task<int> SaveChangesAsync()
        {
            var entries = DbContext.ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntries in entries)
            {
                ((BaseEntity)entityEntries.Entity).ModifiedAt = DateTime.UtcNow;
                if (entityEntries.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntries.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
            DbContext.Entry(entries).State = EntityState.Detached;
            return DbContext.SaveChangesAsync();
        }

    }
}