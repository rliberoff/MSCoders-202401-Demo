using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Dapr.Client;

using Microsoft.SemanticKernel;

namespace MSCoders.Demo.Services.VacationPlanner.Plugins;

internal sealed class FlightsCatalogPlugin(DaprClient daprClient)
{
    [KernelFunction]
    [Description(@"Retrieve flight information, filtered by the following parameters: origin airport, destination airport, range of departure dates, and price range.")]
    [return: Description(@"Flight information with origin and destination airports, departures dates and price.")]
    public async Task<string> FlightsCatalogAsync(
        ILogger logger,
        CancellationToken cancellationToken,
        [Description(@"The name of the departure city. This parameter is optional.")] string? fromAirport = null,
        [Description(@"The name of the destination city or country. This parameter is optional.")] string? toAirport = null,
        [Description(@"The start day date for the flight availability. This parameter is optional.")] int fromDay = 1,
        [Description(@"The start month date for the flight availability. This parameter is optional.")][Range(1, 12)] int fromMonth = 1,
        [Description(@"The end day date for the flight availability. This parameter is optional.")] int? toDay = null,
        [Description(@"The end month date for the flight availability. This parameter is optional.")][Range(1, 12)] int? toMonth = null,
        [Description(@"The minimum price for the search. This parameter is optional.")] decimal? maxPrice = null,
        [Description(@"The maximum price for the search. This parameter is optional.")] decimal? minPrice = null)
    {
        ////logger.LogDebug($@"{nameof(FlightsCatalogAsync)} ==> {nameof(fromAirport)}: '{fromAirport}', {nameof(toAirport)}: '{toAirport}', {nameof(fromDate)}: '{fromDate}', {nameof(toDate)}: '{toDate}', {nameof(maxPrice)}: '{maxPrice}', {nameof(minPrice)}: '{minPrice}'");

        var parameters = string.Join(@"&", new Dictionary<string, object?>
        {
            { "fromAirport", string.IsNullOrWhiteSpace(fromAirport) ? null : fromAirport },
            { "toAirport", string.IsNullOrWhiteSpace(toAirport) ? null : toAirport },
            { "fromDay", fromDay },
            { "fromMonth", fromMonth },
            { "toDay", toDay },
            { "toMonth", toMonth },
            { "maxPrice", maxPrice > 0 ? maxPrice : null },
            { "minPrice", minPrice > 0 ? minPrice : null }
        }.Where(item => item.Value != null).Select(item => $@"{item.Key}={item.Value}"));

        using var httpRequest = daprClient.CreateInvokeMethodRequest(
            HttpMethod.Get,
            @"flights-catalog",
            $@"/catalog/flights?{parameters}");

        using var result = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

        return await result.Content.ReadAsStringAsync(cancellationToken);
    }
}
