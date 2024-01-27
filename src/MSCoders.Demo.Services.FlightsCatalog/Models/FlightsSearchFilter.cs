namespace MSCoders.Demo.Services.FlightsCatalog.Models;

internal sealed record FlightsSearchFilter
{
    public string? FromAirport { get; init; } = null;

    public string? ToAirport { get; init; } = null;

    public DateOnly? FromDate { get; init; } = null;

    public DateOnly? ToDate { get; init; } = null;

    public decimal? MinPrice { get; init; } = null;

    public decimal? MaxPrice { get; init; } = null;
}
