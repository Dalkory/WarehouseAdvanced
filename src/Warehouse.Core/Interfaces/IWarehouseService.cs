using Warehouse.Core.Domain;

namespace Warehouse.Core.Interfaces;

public interface IWarehouseService
{
    Task<IEnumerable<Pallet>> GetPalletsGroupedByExpirationAsync();
    Task<IEnumerable<Pallet>> GetTopPalletsByBoxExpirationAsync(int topCount);
    Task<Pallet?> GetPalletByIdAsync(int id);
    Task AddPalletAsync(Pallet pallet);
    Task AddBoxToPalletAsync(int palletId, Box box);
    Task SeedTestDataAsync();
}
