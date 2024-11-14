using System.Linq.Expressions;

namespace ReadersConnect.Application.BaseInterfaces.Abstractions
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, string? includeProperties = null);
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? expression, string? includeProperties = null);
        Task<IReadOnlyList<T>> GetAllAsync();
        bool Any();
        long Count(Expression<Func<T, bool>> expression);
        Task<int> SaveChangesAsync();
        Task<T> AddAndSaveChangesAsync(T entity);
        T Remove(T entity);
    }
}
