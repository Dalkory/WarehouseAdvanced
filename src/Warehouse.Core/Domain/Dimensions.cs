namespace Warehouse.Core.Domain;

public record Dimensions(double Width, double Height, double Depth)
{
    public double Width { get; } = Width > 0 ? Width : throw new ArgumentException("Width must be positive");
    public double Height { get; } = Height > 0 ? Height : throw new ArgumentException("Height must be positive");
    public double Depth { get; } = Depth > 0 ? Depth : throw new ArgumentException("Depth must be positive");
}
