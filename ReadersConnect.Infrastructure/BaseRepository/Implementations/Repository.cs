using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ReadersConnect.Domain.Models;
using System.Linq.Expressions;
using Mapster;
using ReadersConnect.Application.BaseInterfaces.Abstractions;
using System.Collections.Generic;

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

        #region GetEntity
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, string? includeProperties = null)
        {
            //return await GetAll(expression).FirstOrDefaultAsync();

            IQueryable<T> query = _dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? expression, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        #endregion

        #region AddEntityToDatabase
        public bool Any()
        {
            return DbContext.Set<T>().Any();
        }
        public async Task<T> AddAndSaveChangesAsync(T entity)
        {
            return await DoActionAndSaveChangesAsync(_dbSet.Add, entity);
        }

        #endregion

        #region Remove
        public T Remove(T entity)
        {
            return _dbSet.Remove(entity).Entity;
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

        // PRIVATE METHODS
        private async Task<T> DoActionAndSaveChangesAsync(Func<T, EntityEntry<T>> method, T entity)
        {
            var ent = method(entity).Entity;
            await DbContext.SaveChangesAsync();
            return ent;
        }

    }
}