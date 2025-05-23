using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Warehouse.Core.Application;
using Warehouse.Core.Domain;
using Warehouse.Core.Infrastructure;
using Warehouse.Core.Interfaces;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IRepository<Pallet>, InMemoryPalletRepository>();
        services.AddScoped<IWarehouseService, WarehouseService>();
    })
    .Build();

await RunApplicationAsync(host.Services);
await host.RunAsync();

static async Task RunApplicationAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var warehouseService = scope.ServiceProvider.GetRequiredService<IWarehouseService>();

    await warehouseService.SeedTestDataAsync();
    await DisplayGroupedPalletsAsync(warehouseService);
    await DisplayTopPalletsAsync(warehouseService);
}

static async Task DisplayGroupedPalletsAsync(IWarehouseService service)
{
    var pallets = await service.GetPalletsGroupedByExpirationAsync();
    var groupedPallets = pallets.GroupBy(p => p.ExpirationDate);

    Console.WriteLine("\nGrouped pallets by expiration date (sorted by expiration date and weight):");
    Console.WriteLine(new string('=', 80));

    foreach (var group in groupedPallets.OrderBy(g => g.Key))
    {
        Console.WriteLine($"\nExpiration Date: {group.Key?.ToString("dd.MM.yyyy") ?? "N/A"}");
        Console.WriteLine(new string('-', 80));

        foreach (var pallet in group.OrderBy(p => p.TotalWeight))
        {
            Console.WriteLine(pallet);
        }
    }
}

static async Task DisplayTopPalletsAsync(IWarehouseService service)
{
    const int topCount = 3;
    var topPallets = await service.GetTopPalletsByBoxExpirationAsync(topCount);

    Console.WriteLine($"\nTop {topCount} pallets with boxes having longest expiration dates (sorted by volume):");
    Console.WriteLine(new string('=', 80));

    foreach (var pallet in topPallets)
    {
        Console.WriteLine(pallet);
        Console.WriteLine("Boxes in this pallet (sorted by expiration date):");

        foreach (var box in pallet.Boxes.OrderByDescending(b => b.ExpirationDate))
        {
            Console.WriteLine($"  {box}");
        }

        Console.WriteLine();
    }
}
