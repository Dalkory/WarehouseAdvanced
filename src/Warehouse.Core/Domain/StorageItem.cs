namespace Warehouse.Core.Domain;

public abstract class StorageItem : IEntity<int>
{
    public int Id { get; }
    public Dimensions Dimensions { get; }
    public double Weight { get; }
    public abstract double Volume { get; }

    protected StorageItem(int id, Dimensions dimensions, double weight)
    {
        if (id <= 0)
        {
            throw new WarehouseDomainException("ID must be positive");
        }

        Id = id;
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Weight = weight > 0 ? weight : throw new WarehouseDomainException("Weight must be positive");
    }

    public override bool Equals(object? obj) => obj is StorageItem other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
