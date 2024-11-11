using System.Linq.Expressions;

namespace ReadersConnect.Application.BaseInterfaces.Abstractions
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<List<T>> GetAllAsync();
    }
}
