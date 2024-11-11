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

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}