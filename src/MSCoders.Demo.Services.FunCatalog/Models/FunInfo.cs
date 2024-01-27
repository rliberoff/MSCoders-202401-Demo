namespace MSCoders.Demo.Services.FunCatalog.Models;

public sealed record FunInfo
{
    public string Name { get; init; }

    public decimal Price { get; init; }

    public int Rating { get; init; }

    public string Location { get; init; }
}
