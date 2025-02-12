using EFCorePerformance.Client.Data.Models;
using EFCorePerformance.Client.Fakers;
using Microsoft.EntityFrameworkCore;

namespace EFCorePerformance.Client.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyStore;Trusted_Connection=True;ConnectRetryCount=0")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    public async Task SeedData(
        int numberOfProducts)
    {
        var productFaker = new ProductFaker();
        Products.AddRange(productFaker.Generate(numberOfProducts));

        await SaveChangesAsync();
    }
}
