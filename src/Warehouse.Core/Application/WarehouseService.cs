using Warehouse.Core.Domain;
using Warehouse.Core.Infrastructure;
using Warehouse.Core.Interfaces;

namespace Warehouse.Core.Application;

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Pallet> _palletRepository;

    public WarehouseService(IRepository<Pallet> palletRepository)
    {
        _palletRepository = palletRepository ?? throw new ArgumentNullException(nameof(palletRepository));
    }

    public async Task<IEnumerable<Pallet>> GetPalletsGroupedByExpirationAsync()
    {
        var pallets = await _palletRepository.GetAllAsync();
        return pallets
            .Where(p => p.ExpirationDate.HasValue)
            .OrderBy(p => p.ExpirationDate)
            .ThenBy(p => p.TotalWeight);
    }

    public async Task<IEnumerable<Pallet>> GetTopPalletsByBoxExpirationAsync(int topCount)
    {
        if (topCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(topCount), "Top count must be positive");
        }

        var pallets = await _palletRepository.GetAllAsync();
        return pallets
            .Where(p => p.Boxes.Any())
            .OrderByDescending(p => p.Boxes.Max(b => b.ExpirationDate))
            .Take(topCount)
            .OrderBy(p => p.Volume);
    }

    public async Task<Pallet?> GetPalletByIdAsync(int id) =>
        (await _palletRepository.GetAllAsync()).FirstOrDefault(p => p.Id == id);

    public async Task AddPalletAsync(Pallet pallet) => await _palletRepository.AddAsync(pallet);

    public async Task AddBoxToPalletAsync(int palletId, Box box)
    {
        var pallet = await GetPalletByIdAsync(palletId) ??
            throw new WarehouseDomainException($"Pallet with ID {palletId} not found");

        pallet.AddBox(box);
    }

    public async Task SeedTestDataAsync()
    {
        if (_palletRepository is InMemoryPalletRepository inMemoryRepo)
        {
            inMemoryRepo.SeedTestData();
        }

        await Task.CompletedTask;
    }
}
