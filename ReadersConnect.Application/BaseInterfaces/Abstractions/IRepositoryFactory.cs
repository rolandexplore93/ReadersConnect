namespace ReadersConnect.Application.BaseInterfaces.Abstractions
{
    public interface IRepositoryFactory
    {
        IRepositoryBase<T> GetRepository<T>() where T : class;
    }
}
