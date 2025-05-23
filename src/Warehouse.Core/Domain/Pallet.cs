using Warehouse.Core.Interfaces;

namespace Warehouse.Core.Domain;

public class Pallet : StorageItem, IAggregateRoot
{
    private const double BaseWeight = 30;
    private readonly List<Box> _boxes = new();

    public IReadOnlyCollection<Box> Boxes => _boxes.AsReadOnly();
    public DateOnly? ExpirationDate => _boxes.Count > 0 ? _boxes.Min(b => b.ExpirationDate) : null;
    public override double Volume => Dimensions.Width * Dimensions.Height * Dimensions.Depth + _boxes.Sum(b => b.Volume);
    public double TotalWeight => _boxes.Sum(b => b.Weight) + BaseWeight;

    private Pallet(int id, Dimensions dimensions) : base(id, dimensions, BaseWeight) { }

    public static Pallet Create(int id, Dimensions dimensions) => new(id, dimensions);

    public void AddBox(Box box)
    {
        if (box == null)
        {
            throw new WarehouseDomainException("Box cannot be null");
        }

        if (box.Dimensions.Width > Dimensions.Width || box.Dimensions.Depth > Dimensions.Depth)
        {
            throw new WarehouseDomainException(
                "Box dimensions exceed pallet dimensions");
        }

        _boxes.Add(box);
    }

    public override string ToString()
    {
        return $"Pallet ID: {Id}, Dimensions: {Dimensions.Width}x{Dimensions.Height}x{Dimensions.Depth}, " +
               $"Weight: {TotalWeight}, Volume: {Volume}, " +
               $"Expiration Date: {ExpirationDate?.ToString("dd.MM.yyyy") ?? "N/A"}, " +
               $"Boxes Count: {Boxes.Count}";
    }
}
