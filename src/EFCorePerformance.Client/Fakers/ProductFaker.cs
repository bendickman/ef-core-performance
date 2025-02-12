using Bogus;
using EFCorePerformance.Client.Data.Models;

namespace EFCorePerformance.Client.Fakers;

public class ProductFaker
{
    private Faker<Product>? _faker;
    private int _seed = 123;

    public ProductFaker()
    {
        _faker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Description, f => f.Random.Words(10))
            .RuleFor(p => p.Rating, f => f.Random.Int(1, 100))
            .RuleFor(p => p.CreatedDateTime, f => f.Date.Past());
    }

    public IEnumerable<Product> Generate(
        int count,
        int? seed = null)
    {
        _faker!.UseSeed(seed ?? _seed);

        return _faker.Generate(count);
    }
}
