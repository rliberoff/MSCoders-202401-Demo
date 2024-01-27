namespace MSCoders.Demo.Services.FlightsCatalog.Models;

public sealed record FlightInfo
{
    public DateOnly FromDate { get; init; }

    public DateOnly ToDate { get; init; }

    public decimal Price { get; init; }

    public string FromAirport { get; init; }

    public string ToAirport { get; init; }
}
