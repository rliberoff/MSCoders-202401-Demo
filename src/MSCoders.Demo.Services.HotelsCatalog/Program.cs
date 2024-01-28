using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.Extensions.Logging.ApplicationInsights;

using MSCoders.Demo.Services.Common;
using MSCoders.Demo.Services.HotelsCatalog.Models;

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

/* Application Middleware Configuration */

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet(@"/catalog/hotels", Results<Ok<IEnumerable<HotelInfo>>, NotFound<NotFoundMessage>> (
    string? location = null,
    int fromDay = 1,
    [Range(0, 12)] int fromMonth = 1,
    int? toDay = null,
    [Range(0, 12)] int? toMonth = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    [Range(0, 5)] int minRating = 0,
    [Range(0, 5)] int maxRating = 5)
    =>
{
    var currentYear = DateTime.Now.Year;

    var hotels = HotelsCatalog.DemoHotelsCatalog.SearchHotels(new HotelsSearchFilter()
    {
        Location = location,
        FromDate = new DateOnly(currentYear, fromMonth, fromDay),
        ToDate = new DateOnly(currentYear, toMonth ?? fromMonth, toDay ?? DateTime.DaysInMonth(currentYear, fromMonth)),
        MinPrice = minPrice,
        MaxPrice = maxPrice,
        MinRating = minRating,
        MaxRating = maxRating,
    });

    return hotels.Any()
        ? TypedResults.Ok(hotels)
        : TypedResults.NotFound(new NotFoundMessage
        {
            Message = "Not Found: No hotels found matching the provided criteria!",
        });
})
.WithName(@"SearchHotels")
.WithOpenApi();

app.Run();
