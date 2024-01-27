namespace MSCoders.Demo.Services.HotelsCatalog.Models;

public sealed record HotelInfo
{
    public string Name { get; init; }
    
    public DateOnly AvailableFromDate { get; init; }

    public DateOnly AvailableToDate { get; init; }

    public decimal Price { get; init; }

    public int Rating { get; init; }

    public string Location { get; init; }
}
