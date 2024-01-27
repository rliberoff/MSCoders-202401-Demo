using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Dapr.Client;

using Microsoft.SemanticKernel;

namespace MSCoders.Demo.Services.VacationPlanner.Plugins;

internal sealed class FunCatalogPlugin(DaprClient daprClient)
{
    [KernelFunction]
    [Description(@"Gets information for fun, cultural or leisure activities in a location or city, in a range dates, rating (in stars from zero to five), and price range.")]
    [return: Description(@"Flight information with origin and destination airports, departures dates and price.")]
    public async Task<string> HotelsCatalogAsync(
       ILogger logger,
       CancellationToken cancellationToken,
       [Description(@"The name of the city or location for the fun, cultural or leisure activities.")] string location,
       [Description(@"The minimum price for the search. This parameter is optional.")] decimal? minPrice = null,
       [Description(@"The maximum price for the search. This parameter is optional.")] decimal? maxPrice = null,
       [Description(@"The minimum star rating of the activity, ranging from 0 to 5. This parameter is optional.")][Range(0, 5)] int minRating = 0,
       [Description(@"The maximum star rating of the activity, ranging from 0 to 5. This parameter is optional.")][Range(0, 5)] int maxRating = 5)
    {
        ////logger.LogDebug($@"{nameof(HotelsCatalogPlugin)} ==> {nameof(location)}: '{location}', {nameof(fromDay)}: '{fromDay}', {nameof(toDay)}: {toDay}, {nameof(fromMonth)}:{fromMonth}, {nameof(toMonth)}:{toMonth}, {nameof(fromPrice)}: {fromPrice}, {nameof(toPrice)}: {toPrice}, {nameof(minRating)}: {minRating}, {nameof(maxRating)}: {maxRating}");

        var parameters = string.Join(@"&", new Dictionary<string, object?>
        {
            { "minRating", minRating },
            { "maxRating", maxRating },
            { "location", location },
            { "minPrice", minPrice > 0 ? minPrice : null },
            { "maxPrice", maxPrice > 0 ? maxPrice : null }
        }.Where(item => item.Value != null).Select(item => $@"{item.Key}={item.Value}"));

        using var httpRequest = daprClient.CreateInvokeMethodRequest(
            HttpMethod.Get,
            @"fun-catalog",
            $@"/catalog/fun?{parameters}");

        using var result = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

        var response = await result.Content.ReadAsStringAsync(cancellationToken);

        return response;
    }
}
