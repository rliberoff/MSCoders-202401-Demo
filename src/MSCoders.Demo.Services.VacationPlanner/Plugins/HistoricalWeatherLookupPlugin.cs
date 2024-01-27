using System.ComponentModel;

using Dapr.Client;

using Microsoft.SemanticKernel;

namespace MSCoders.Demo.Services.VacationPlanner.Plugins;

internal sealed class HistoricalWeatherLookupPlugin(DaprClient daprClient)
{
    [KernelFunction]
    [Description(@"Get the historical weather for a location for a month. Make sure and pass in the month of the year the user requested. Make sure and pass the valid name of the location requested by the user.")]
    [return: Description(@"The weather temperatures will be returned in Celsius. Not Found will be returned if the name of the location or the month of the year are not valid.")]
    public async Task<string> HistoricalWeatherLookupAsync(
        ILogger logger,
        CancellationToken cancellationToken,
        [Description(@"The name of the location to lookup historical weather for. This should be a string, not JSON.")] string location,
        [Description(@"The integer month of the year to lookup historical weather for")] int monthOfYear)
    {
        logger.LogDebug($@"{nameof(HistoricalWeatherLookupAsync)} ==> {nameof(location)} : {location}, {nameof(monthOfYear)}: {monthOfYear}");

        try
        {
            using var httpRequest = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, @"historical-weather-lookup", $@"historical-weather-lookup?location={location}&monthOfYear={monthOfYear}");

            using var result = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}
