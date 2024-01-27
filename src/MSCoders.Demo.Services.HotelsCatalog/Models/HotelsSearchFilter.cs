using System.ComponentModel.DataAnnotations;

namespace MSCoders.Demo.Services.HotelsCatalog.Models;

internal sealed record HotelsSearchFilter
{
    public string? Location { get; init; } = null;

    public DateOnly? FromDate { get; init; } = null;

    public DateOnly? ToDate { get; init; } = null;

    public decimal? MinPrice { get; init; } = null;

    public decimal? MaxPrice { get; init; } = null;

    [Range(0, 5)]
    public int MinRating { get; init; } = 0;

    [Range(0, 5)]
    public int MaxRating { get; init; } = 5;
}
