using System.ComponentModel.DataAnnotations;

namespace MSCoders.Demo.Services.FunCatalog.Models;

internal sealed record FunSearchFilter
{
    public string? Location { get; init; } = null;

    public decimal? MinPrice { get; init; } = null;

    public decimal? MaxPrice { get; init; } = null;

    [Range(0, 5)]
    public int MinRating { get; init; } = 0;

    [Range(0, 5)]
    public int MaxRating { get; init; } = 5;
}
