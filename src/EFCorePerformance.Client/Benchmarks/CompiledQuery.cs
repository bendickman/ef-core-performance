using BenchmarkDotNet.Attributes;
using EFCorePerformance.Client.Data;
using EFCorePerformance.Client.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCorePerformance.Client.Benchmarks;

[MemoryDiagnoser]
public class CompiledQuery
{
    private static Func<ApplicationDbContext, IAsyncEnumerable<Product>> _compiledQuery
        => EF.CompileAsyncQuery((ApplicationDbContext dbContext) => dbContext.Products.Where(p => p.Url.StartsWith("https://")));

    private ApplicationDbContext _dbContext;

    [Params(1, 10)]
    public int NumberOfProducts { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        using var dbContext = new ApplicationDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.SeedData(NumberOfProducts);

        _dbContext = new ApplicationDbContext();
    }

    [Benchmark]
    public async ValueTask<string> WithCompiledQuery()
    {
        var selectedProductIds = new List<int>();

        await foreach (var product in _compiledQuery(_dbContext))
        {
            selectedProductIds.Add(product.Id);
        }

        return string.Join(", ", selectedProductIds);
    }

    [Benchmark]
    public async ValueTask<string> WithoutCompiledQuery()
    {
        var selectedRatedProductIds = new List<int>();

        await foreach (var product in _dbContext.Products.Where(p => p.Url.StartsWith("https://")).AsAsyncEnumerable())
        {
            selectedRatedProductIds.Add(product.Id);
        }

        return string.Join(", ", selectedRatedProductIds);
    }

    [GlobalCleanup]
    public ValueTask CleanUp() => _dbContext.DisposeAsync();
}
