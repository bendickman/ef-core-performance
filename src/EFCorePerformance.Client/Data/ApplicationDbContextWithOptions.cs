using EFCorePerformance.Client.Data.Models;
using EFCorePerformance.Client.Fakers;
using Microsoft.EntityFrameworkCore;

namespace EFCorePerformance.Client.Data;

public class ApplicationDbContextWithOptions : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ApplicationDbContextWithOptions(
        DbContextOptions options) : base(options)
    {
    }

    public async Task SeedData(
        int numberOfProducts)
    {
        var productFaker = new ProductFaker();
        Products.AddRange(productFaker.Generate(numberOfProducts));

        await SaveChangesAsync();
    }
}
