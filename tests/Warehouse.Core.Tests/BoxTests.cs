using FluentAssertions;
using Warehouse.Core.Domain;

public class BoxTests
{
    [Fact]
    public void CreateWithProductionDate_ValidData_ShouldSetExpirationDate()
    {
        // Arrange
        var productionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var expectedExpirationDate = productionDate.AddDays(100);

        // Act
        var box = Box.CreateWithProductionDate(1, new Dimensions(10, 10, 10), 5, productionDate);

        // Assert
        box.ProductionDate.Should().Be(productionDate);
        box.ExpirationDate.Should().Be(expectedExpirationDate);
    }

    [Fact]
    public void CreateWithExpirationDate_ValidData_ShouldSetExpirationDate()
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20));

        // Act
        var box = Box.CreateWithExpirationDate(1, new Dimensions(10, 10, 10), 5, expirationDate);

        // Assert
        box.ExpirationDate.Should().Be(expirationDate);
        box.ProductionDate.Should().BeNull();
    }

    [Fact]
    public void CreateWithProductionDate_FutureDate_ShouldThrow()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        // Act & Assert
        Assert.Throws<WarehouseDomainException>(() =>
            Box.CreateWithProductionDate(1, new Dimensions(10, 10, 10), 5, futureDate));
    }

    [Fact]
    public void CreateWithExpirationDate_PastDate_ShouldThrow()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

        // Act & Assert
        Assert.Throws<WarehouseDomainException>(() =>
            Box.CreateWithExpirationDate(1, new Dimensions(10, 10, 10), 5, pastDate));
    }

    [Fact]
    public void Volume_ShouldCalculateCorrectly()
    {
        // Arrange
        var box = Box.CreateWithProductionDate(1, new Dimensions(2, 3, 4), 5, DateOnly.FromDateTime(DateTime.Now));

        // Act & Assert
        box.Volume.Should().Be(24);
    }
}
