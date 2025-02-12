namespace EFCorePerformance.Client.Data.Models;

public class Product
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedDateTime { get; init; }

    public DateTime? UpdatedDateTime { get; init; }

    public int Rating { get; init; }

    public string Url { get; init; } = string.Empty;
}
