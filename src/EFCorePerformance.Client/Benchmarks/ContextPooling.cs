using BenchmarkDotNet.Attributes;
using EFCorePerformance.Client.Data;
using EFCorePerformance.Client.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCorePerformance.Client.Benchmarks;

[MemoryDiagnoser]
public class ContextPooling
{
    private DbContextOptions<ApplicationDbContextWithOptions> _options;
    private PooledDbContextFactory<ApplicationDbContextWithOptions> _pooledDbContextFactory;

    [Params(1)]
    public int NumberOfProducts { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContextWithOptions>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyStore;Trusted_Connection=True;ConnectRetryCount=0")
            .Options;

        using var context = new ApplicationDbContextWithOptions(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(NumberOfProducts);

        _pooledDbContextFactory = new PooledDbContextFactory<ApplicationDbContextWithOptions>(_options);
    }

    [Benchmark]
    public async Task<List<Product>> WithoutContextPooling()
    {
        using var context = new ApplicationDbContextWithOptions(_options);

        return await context.Products.ToListAsync();
    }

    [Benchmark]
    public async Task<List<Product>> WithContextPooling()
    {
        using var context = _pooledDbContextFactory.CreateDbContext();

        return await context.Products.ToListAsync();
    }
}
