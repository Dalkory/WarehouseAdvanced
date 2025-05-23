namespace Warehouse.Core.Domain
{
    public interface IEntity<TId> where TId : struct
    {
        TId Id { get; }
    }
}
