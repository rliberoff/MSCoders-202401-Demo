using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.Extensions.Logging.ApplicationInsights;

using MSCoders.Demo.Services.Common;
using MSCoders.Demo.Services.FlightsCatalog.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

var isDevelopment = builder.Environment.IsDevelopment();

/* Load Configuration */

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

/* Logging Configuration */

if (isDevelopment)
{
    builder.Logging.AddConsole();

    if (Debugger.IsAttached)
    {
        builder.Logging.AddDebug();
    }
}

var applicationInsightsConnectionString = builder.Configuration.GetConnectionString(@"ApplicationInsights");

builder.Logging.AddApplicationInsights((telemetryConfiguration) => telemetryConfiguration.ConnectionString = applicationInsightsConnectionString, (_) => { })
               .AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace)
               ;

/* Application Services */

builder.AddServiceDefaults();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration)
                .AddRouting()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddHealthChecks()
                ;

/* Define Application Types */

var flightsCatalog = new FlightCatalog()
{
    // Madrid to Cancun
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 09, 30),
        Price = 1000,
        FromAirport = "Madrid",
        ToAirport = "Cancun",
    },

    // Madrid to Dominican Rep.
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 09, 30),
        Price = 500,
        FromAirport = "Madrid",
        ToAirport = "Dominican Republic"
    },

    // Madrid to Punta Cana
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 09, 30),
        Price = 800,
        FromAirport = "Madrid",
        ToAirport = "Punta Cana",
    },

    // Madrid to Bahamas
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 09, 30),
        Price = 900,
        FromAirport = "Madrid",
        ToAirport = "Bahamas",
    },

    // Madrid to Switzerland
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 11, 30),
        Price = 2000,
        FromAirport = "Madrid",
        ToAirport = "Switzerland",
    },

    // Madrid to Canada
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 02, 1),
        ToDate = new DateOnly(2024, 11, 30),
        Price = 1500,
        FromAirport = "Madrid",
        ToAirport = "Canada",
    },

    // Madrid to Munich
    new FlightInfo
    {
        FromDate = new DateOnly(2024, 06, 1),
        ToDate = new DateOnly(2024, 09, 30),
        Price = 600,
        FromAirport = "Madrid",
        ToAirport = "Munich",
    },
};

/* Application Middleware Configuration */

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet(@"/catalog/flights", Results<Ok<IEnumerable<FlightInfo>>, NotFound<NotFoundMessage>> (
    string? fromAirport = null,
    string? toAirport = null,
    int fromDay = 1,
    [Range(0, 12)] int fromMonth = 1,
    int? toDay = null,
    [Range(0, 12)] int? toMonth = null,
    decimal? minPrice = null,
    decimal? maxPrice = null)
    =>
{
    var currentYear = DateTime.Now.Year;

    var flights = flightsCatalog.SearchFlights(new FlightsSearchFilter()
    {
        FromAirport = fromAirport,
        ToAirport = toAirport,
        FromDate = new DateOnly(currentYear, fromMonth, fromDay),
        ToDate = new DateOnly(currentYear, toMonth ?? fromMonth, toDay ?? DateTime.DaysInMonth(currentYear, fromMonth)),
        MinPrice = minPrice,
        MaxPrice = maxPrice,
    });

    return flights.Any()
        ? TypedResults.Ok(flights)
        : TypedResults.NotFound(new NotFoundMessage
        {
            Message = @"Not Found: No flights found matching the provided criteria!",
        });
})
.WithName(@"SearchFlights")
.WithOpenApi();

app.Run();
