using FluentAssertions;
using Warehouse.Core.Domain;

namespace Warehouse.Core.Tests;

public class PalletTests
{
    [Fact]
    public void CreatePallet_WithValidDimensions_ShouldSucceed()
    {
        // Arrange
        var dimensions = new Dimensions(100, 100, 100);

        // Act
        var pallet = Pallet.Create(1, dimensions);

        // Assert
        pallet.Should().NotBeNull();
        pallet.Dimensions.Should().Be(dimensions);
        pallet.Weight.Should().Be(30);
        pallet.Boxes.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, 100, 100)]
    [InlineData(100, 0, 100)]
    [InlineData(100, 100, 0)]
    [InlineData(-1, 100, 100)]
    public void CreatePallet_WithInvalidDimensions_ShouldThrow(double w, double h, double d)
    {
        // Act
        Action act = () => Pallet.Create(1, new Dimensions(w, h, d));

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*positive*");
    }

    [Fact]
    public void AddBox_WithValidDimensions_ShouldAddBox()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));
        var box = Box.CreateWithProductionDate(1, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now));

        // Act
        pallet.AddBox(box);

        // Assert
        pallet.Boxes.Should().Contain(box);
        pallet.TotalWeight.Should().Be(40);
        pallet.Volume.Should().Be(100 * 100 * 100 + 50 * 50 * 50);
    }

    [Fact]
    public void AddBox_WithInvalidDimensions_ShouldThrow()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));
        var box = Box.CreateWithProductionDate(1, new Dimensions(150, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now));

        // Act & Assert
        Assert.Throws<WarehouseDomainException>(() => pallet.AddBox(box));
    }

    [Fact]
    public void AddBox_NullBox_ShouldThrow()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));

        // Act & Assert
        Assert.Throws<WarehouseDomainException>(() => pallet.AddBox(null!));
    }

    [Fact]
    public void ExpirationDate_WithMultipleBoxes_ShouldReturnMinExpiration()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));
        var box1 = Box.CreateWithExpirationDate(1, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now.AddDays(10)));
        var box2 = Box.CreateWithExpirationDate(2, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now.AddDays(20)));

        // Act
        pallet.AddBox(box1);
        pallet.AddBox(box2);

        // Assert
        pallet.ExpirationDate.Should().Be(box1.ExpirationDate);
    }

    [Fact]
    public void ExpirationDate_WithNoBoxes_ShouldReturnNull()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));

        // Act & Assert
        pallet.ExpirationDate.Should().BeNull();
    }
}
