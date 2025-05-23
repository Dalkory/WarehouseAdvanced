using Warehouse.Core.Domain;
using Warehouse.Core.Interfaces;

namespace Warehouse.Core.Infrastructure;

public class InMemoryPalletRepository : IRepository<Pallet>
{
    private const int MinPalletDimension = 100;
    private const int MaxPalletDimension = 200;
    private const int MinBoxDimension = 20;
    private const int DefaultPalletWeight = 30;
    private const int BoxExpirationDays = 100;
    private const int MinBoxWeight = 1;
    private const int MaxBoxWeight = 20;
    private const int MinBoxesPerPallet = 1;
    private const int MaxBoxesPerPallet = 5;
    private const int MinExpirationDays = 1;
    private const int MaxExpirationDays = 200;
    private const int MaxPastDaysForProductionDate = 50;

    private readonly List<Pallet> _pallets = new();
    private readonly object _lock = new();

    public Task<IEnumerable<Pallet>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Pallet>>(_pallets.ToList());

    public Task AddAsync(Pallet entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        lock (_lock)
        {
            if (_pallets.Any(p => p.Id == entity.Id))
            {
                throw new WarehouseDomainException($"Pallet with ID {entity.Id} already exists");
            }

            _pallets.Add(entity);
        }

        return Task.CompletedTask;
    }

    public void SeedTestData()
    {
        _pallets.Clear();
        var random = new Random();

        for (int i = 1; i <= 10; i++)
        {
            var palletDimensions = new Dimensions(
                random.Next(MinPalletDimension, MaxPalletDimension),
                random.Next(MinPalletDimension, MaxPalletDimension),
                random.Next(MinPalletDimension, MaxPalletDimension));

            var pallet = Pallet.Create(i, palletDimensions);

            int boxCount = random.Next(MinBoxesPerPallet, MaxBoxesPerPallet + 1);
            for (int j = 1; j <= boxCount; j++)
            {
                try
                {
                    var boxId = i * 100 + j;
                    var boxDimensions = new Dimensions(
                        random.Next(MinBoxDimension, (int)pallet.Dimensions.Width),
                        random.Next(MinBoxDimension, (int)pallet.Dimensions.Height),
                        random.Next(MinBoxDimension, (int)pallet.Dimensions.Depth));

                    var weight = random.Next(MinBoxWeight, MaxBoxWeight + 1);

                    if (random.Next(2) == 0)
                    {
                        var productionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-random.Next(0, MaxPastDaysForProductionDate)));
                        pallet.AddBox(Box.CreateWithProductionDate(boxId, boxDimensions, weight, productionDate));
                    }
                    else
                    {
                        var expirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(random.Next(MinExpirationDays, MaxExpirationDays)));
                        pallet.AddBox(Box.CreateWithExpirationDate(boxId, boxDimensions, weight, expirationDate));
                    }
                }
                catch (WarehouseDomainException ex)
                {
                    Console.WriteLine($"Skipping invalid box: {ex.Message}");
                }
            }

            _pallets.Add(pallet);
        }
    }
}