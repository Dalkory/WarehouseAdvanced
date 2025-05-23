namespace Warehouse.Core.Interfaces;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
}
