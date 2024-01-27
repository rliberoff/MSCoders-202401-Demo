using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Dapr.Client;

using Microsoft.SemanticKernel;

namespace MSCoders.Demo.Services.VacationPlanner.Plugins;

internal sealed class HotelsCatalogPlugin(DaprClient daprClient)
{
    [KernelFunction]
    [Description(@"Gets information for hotels from the following parameters: location or city, range dates for the check-in or arrival, rating (in stars from zero to five), and price range.")]
    [return: Description(@"Flight information with origin and destination airports, departures dates and price.")]
    public async Task<string> HotelsCatalogAsync(
        ILogger logger,
        CancellationToken cancellationToken,
        [Description(@"The name of the city or location of the hotel. This parameter is optional.")] string? location = null,
        [Description(@"The start day for the hotel arrival or check-in. This parameter is optional.")] int fromDay = 1,
        [Description(@"The start month for the hotel arrival or check-in. This parameter is optional.")][Range(1, 12)] int fromMonth = 1,
        [Description(@"The end day for the hotel arrival or check-in. This parameter is optional.")] int? toDay = null,
        [Description(@"The end month for the hotel arrival or check-in. This parameter is optional.")][Range(1, 12)] int? toMonth = null,
        [Description(@"The minimum price for the search. This parameter is optional.")] decimal? minPrice = null,
        [Description(@"The maximum price for the search. This parameter is optional.")] decimal? maxPrice = null,
        [Description(@"The minimum star rating for the hotel, ranging from 0 to 5. This parameter is optional.")][Range(0, 5)] int minRating = 0,
        [Description(@"The maximum star rating for the hotel, ranging from 0 to 5. This parameter is optional.")][Range(0, 5)] int maxRating = 5)
    {
        ////logger.LogDebug($@"{nameof(HotelsCatalogPlugin)} ==> {nameof(location)}: '{location}', {nameof(fromDay)}: '{fromDay}', {nameof(toDay)}: {toDay}, {nameof(fromMonth)}:{fromMonth}, {nameof(toMonth)}:{toMonth}, {nameof(fromPrice)}: {fromPrice}, {nameof(toPrice)}: {toPrice}, {nameof(minRating)}: {minRating}, {nameof(maxRating)}: {maxRating}");

        var parameters = string.Join(@"&", new Dictionary<string, object?>
        {
            { "minRating", minRating },
            { "maxRating", maxRating },
            { "location", location },
            { "fromDay", fromDay },
            { "fromMonth", fromMonth },
            { "toDay", toDay },
            { "toMonth", toMonth },
            { "minPrice", minPrice > 0 ? minPrice : null },
            { "maxPrice", maxPrice > 0 ? maxPrice : null }
        }.Where(item => item.Value != null).Select(item => $@"{item.Key}={item.Value}"));

        using var httpRequest = daprClient.CreateInvokeMethodRequest(
            HttpMethod.Get,
            @"hotels-catalog",
            $@"/catalog/hotels?{parameters}");

        using var httpResponse = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        return response;
    }
}
