using BenchmarkDotNet.Attributes;
using EFCorePerformance.Client.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCorePerformance.Client.Benchmarks;

[MemoryDiagnoser]
public class AverageProductRating
{
    [Params(1000)]
    public int NumberOfProducts { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        using var dbContext = new ApplicationDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.SeedData(NumberOfProducts);
    }

    [Benchmark]
    public async Task<double> LoadAllProducts()
    {
        var total = 0;
        var count = 0;
        using var dbContext = new ApplicationDbContext();
        await foreach (var product in dbContext.Products.AsAsyncEnumerable())
        {
            total += product.Rating;
            count++;
        }

        return (double)total / count;
    }

    [Benchmark]
    public async Task<double> LoadAllProductsNoTracking()
    {
        var total = 0;
        var count = 0;
        using var dbContext = new ApplicationDbContext();
        await foreach (var product in dbContext.Products.AsNoTracking().AsAsyncEnumerable())
        {
            total += product.Rating;
            count++;
        }

        return (double)total / count;
    }

    [Benchmark]
    public async Task<double> ProjectOnlyRating()
    {
        var total = 0;
        var count = 0;
        using var dbContext = new ApplicationDbContext();
        await foreach (var rating in dbContext.Products.Select(p => p.Rating).AsAsyncEnumerable())
        {
            total += rating;
            count++;
        }

        return (double)total / count;
    }

    [Benchmark(Baseline = true)]
    public async Task<double> CalculationInDatabase()
    {
        using var dbContext = new ApplicationDbContext();

        return await dbContext.Products.AverageAsync(p => p.Rating);
    }
}
