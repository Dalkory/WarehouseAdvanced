using FluentAssertions;
using Moq;
using Warehouse.Core.Application;
using Warehouse.Core.Domain;
using Warehouse.Core.Interfaces;

namespace Warehouse.Core.Tests;

public class WarehouseServiceTests
{
    private readonly Mock<IRepository<Pallet>> _mockRepository;
    private readonly WarehouseService _service;

    public WarehouseServiceTests()
    {
        _mockRepository = new Mock<IRepository<Pallet>>();
        _service = new WarehouseService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetPalletsGroupedByExpirationAsync_ShouldReturnCorrectlyGroupedAndSortedPallets()
    {
        // Arrange
        var pallets = CreateTestPallets();
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pallets);

        // Act
        var result = await _service.GetPalletsGroupedByExpirationAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeInAscendingOrder(p => p.ExpirationDate);

        var grouped = result.GroupBy(p => p.ExpirationDate);
        foreach (var group in grouped)
        {
            group.Should().BeInAscendingOrder(p => p.TotalWeight);
        }
    }

    [Fact]
    public async Task GetTopPalletsByBoxExpirationAsync_ShouldReturnTop3SortedByVolume()
    {
        // Arrange
        var pallets = CreateTestPallets();
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(pallets);

        var allPallets = pallets.Where(p => p.Boxes.Any()).ToList();
        var allMaxExpirations = allPallets
            .Select(p => p.Boxes.Max(b => b.ExpirationDate))
            .OrderByDescending(d => d)
            .ToList();
        var top3Expirations = allMaxExpirations.Take(3).ToList();
        var minTopExpiration = top3Expirations.Any() ? top3Expirations.Min() : (DateOnly?)null;

        // Act
        var result = await _service.GetTopPalletsByBoxExpirationAsync(3);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInAscendingOrder(p => p.Volume);

        if (minTopExpiration.HasValue)
        {
            var resultExpirations = result.Select(p => p.Boxes.Max(b => b.ExpirationDate)).ToList();
            resultExpirations.Should().OnlyContain(e => e >= minTopExpiration);
        }
    }

    [Fact]
    public async Task AddBoxToPalletAsync_ValidBox_ShouldAddToPallet()
    {
        // Arrange
        var pallet = Pallet.Create(1, new Dimensions(100, 100, 100));
        var box = Box.CreateWithProductionDate(1, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now));

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Pallet> { pallet });

        // Act
        await _service.AddBoxToPalletAsync(1, box);

        // Assert
        pallet.Boxes.Should().Contain(box);
    }

    [Fact]
    public async Task AddBoxToPalletAsync_NonexistentPallet_ShouldThrow()
    {
        // Arrange
        var box = Box.CreateWithProductionDate(1, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now));

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Pallet>());

        // Act & Assert
        await Assert.ThrowsAsync<WarehouseDomainException>(() =>
            _service.AddBoxToPalletAsync(1, box));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetTopPalletsByBoxExpirationAsync_InvalidTopCount_ShouldThrow(int topCount)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _service.GetTopPalletsByBoxExpirationAsync(topCount));
    }

    private List<Pallet> CreateTestPallets()
    {
        var pallet1 = Pallet.Create(1, new Dimensions(100, 100, 100));
        pallet1.AddBox(Box.CreateWithExpirationDate(101, new Dimensions(50, 50, 50), 10, DateOnly.FromDateTime(DateTime.Now.AddDays(30))));
        pallet1.AddBox(Box.CreateWithProductionDate(102, new Dimensions(60, 60, 60), 15, DateOnly.FromDateTime(DateTime.Now.AddDays(-20))));

        var pallet2 = Pallet.Create(2, new Dimensions(120, 120, 120));
        pallet2.AddBox(Box.CreateWithExpirationDate(201, new Dimensions(70, 70, 70), 20, DateOnly.FromDateTime(DateTime.Now.AddDays(50))));

        var pallet3 = Pallet.Create(3, new Dimensions(150, 150, 150));
        pallet3.AddBox(Box.CreateWithExpirationDate(301, new Dimensions(80, 80, 80), 25, DateOnly.FromDateTime(DateTime.Now.AddDays(10))));
        pallet3.AddBox(Box.CreateWithProductionDate(302, new Dimensions(90, 90, 90), 30, DateOnly.FromDateTime(DateTime.Now.AddDays(-10))));

        var pallet4 = Pallet.Create(4, new Dimensions(200, 200, 200));

        return new List<Pallet> { pallet1, pallet2, pallet3, pallet4 };
    }
}
