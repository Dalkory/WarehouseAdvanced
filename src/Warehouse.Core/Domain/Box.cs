namespace Warehouse.Core.Domain;

public class Box : StorageItem
{
    private DateOnly? _productionDate;
    private DateOnly? _expirationDate;

    public DateOnly? ProductionDate
    {
        get => _productionDate;
        private set
        {
            _productionDate = value;
            _expirationDate = value?.AddDays(100);
        }
    }

    public DateOnly? ExpirationDate
    {
        get => _expirationDate;
        private set
        {
            _expirationDate = value;
            _productionDate = null;
        }
    }

    public override double Volume => Dimensions.Width * Dimensions.Height * Dimensions.Depth;

    private Box(int id, Dimensions dimensions, double weight)
        : base(id, dimensions, weight) { }

    public static Box CreateWithProductionDate(int id, Dimensions dimensions, double weight, DateOnly productionDate)
    {
        if (productionDate > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new WarehouseDomainException("Production date cannot be in the future");
        }

        return new Box(id, dimensions, weight)
        {
            ProductionDate = productionDate
        };
    }

    public static Box CreateWithExpirationDate(int id, Dimensions dimensions, double weight, DateOnly expirationDate)
    {
        if (expirationDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new WarehouseDomainException("Expiration date cannot be in the past");
        }

        return new Box(id, dimensions, weight)
        {
            ExpirationDate = expirationDate
        };
    }

    public override string ToString()
    {
        return $"Box ID: {Id}, Dimensions: {Dimensions.Width}x{Dimensions.Height}x{Dimensions.Depth}, " +
               $"Weight: {Weight}, Volume: {Volume}, " +
               $"Production Date: {ProductionDate?.ToString("dd.MM.yyyy") ?? "N/A"}, " +
               $"Expiration Date: {ExpirationDate?.ToString("dd.MM.yyyy") ?? "N/A"}";
    }
}
