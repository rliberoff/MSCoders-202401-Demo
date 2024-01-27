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

/* Define Application Types */

var hotelsCatalog = new HotelsCatalog()
{
    // Cancun
    new HotelInfo
    {
        Name = @"Hotel Rivemar",
        Location = @"Cancun",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 40,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Hotel Imperial Laguna Faranda",
        Location = @"Cancun",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 68,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Aloft Cancún",
        Location = @"Cancun",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 119,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Cancun Bay Resort",
        Location = @"Cancun",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 267,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"JW Marriott Cancun Resort & SPA",
        Location = @"Cancun",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 359,
        Rating = 5,
    },

    // Dominican Rep.
    new HotelInfo
    {
        Name = @"Hamilton",
        Location = @"República Dominicana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 58,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Cataleya",
        Location = @"República Dominicana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 75,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Tropical",
        Location = @"República Dominicana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 140,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Renaissance Santo Domingo Jaragua",
        Location = @"República Dominicana",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 210,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Sublime Sabana",
        Location = @"República Dominicana",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 388,
        Rating = 5,
    },

    // Punta Cana
    new HotelInfo
    {
        Name = @"Hotel 2 Garden",
        Location = @"Punta Cana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 37,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Capriccio Mare",
        Location = @"Punta Cana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 100,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Hotel The Westin Puntacana ",
        Location = @"Punta Cana",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 410,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Royalton Punta Cana",
        Location = @"Punta Cana",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 526,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Occidental Punta Cana",
        Location = @"Punta Cana",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 912,
        Rating = 5,
    },

    // Bahamas
    new HotelInfo
    {
        Name = @"Hotel The Towne",
        Location = @"Bahamas",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 142,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Bell Channel Inn",
        Location = @"Bahamas",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 147,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Viva Wyndham Fortuna Beach",
        Location = @"Bahamas",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 242,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Hilton At Resorts World Bimini",
        Location = @"Bahamas",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 442,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Goldwynn Resort & Residences",
        Location = @"Bahamas",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 723,
        Rating = 5,
    },

    // Switzerland
    new HotelInfo
    {
        Name = @"Lugano Paradiso",
        Location = @"Switzerland",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 113,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Genève Centre Nations",
        Location = @"Switzerland",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 134,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Central Hotel Wolter",
        Location = @"Switzerland",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 265,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Hotel Bristol Geneva",
        Location = @"Switzerland",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 313,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Hotel Beau-Rivage, Geneva",
        Location = @"Switzerland",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 621,
        Rating = 5,
    },

    // Madrid
    new HotelInfo
    {
        Name = @"Madrid Centro las Ventas",
        Location = @"Madrid",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 80,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Hotel Mayerling",
        Location = @"Madrid",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 130,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Hotel Principe Pio",
        Location = @"Madrid",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 151,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Only YOU Hotel Atocha",
        Location = @"Madrid",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 235,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"BLESS Madrid",
        Location = @"Madrid",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 617,
        Rating = 5,
    },

    // Munich
    new HotelInfo
    {
        Name = @"Muenchen City Sued",
        Location = @"Munich",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 63,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Hotel München Messe",
        Location = @"Munich",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 95,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Mercure Hotel München Altstadt",
        Location = @"Munich",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 169,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Yours Truly",
        Location = @"Munich",
        AvailableFromDate = new DateOnly(2024, 02, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 214,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Hotel Vier Jahreszeiten Kempinski Munich",
        Location = @"Munich",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 393,
        Rating = 5,
    },

    // Canada
    new HotelInfo
    {
        Name = @"Super 8 Sicamous",
        Location = @"Canada",
        AvailableFromDate = new DateOnly(2023, 12, 01),
        AvailableToDate = new DateOnly(2024, 05, 31),
        Price = 69,
        Rating = 1,
    },
    new HotelInfo
    {
        Name = @"Microtel Inn & Suites by Wyndham Timmins",
        Location = @"Canada",
        AvailableFromDate = new DateOnly(2024, 06, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 144,
        Rating = 2,
    },
    new HotelInfo
    {
        Name = @"Holiday Inn Kingston Waterfront",
        Location = @"Canada",
        AvailableFromDate = new DateOnly(2024, 04, 1),
        AvailableToDate = new DateOnly(2024, 10, 31),
        Price = 237,
        Rating = 3,
    },
    new HotelInfo
    {
        Name = @"Hotel Fairmont Banff Springs",
        Location = @"Canada",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 11, 30),
        Price = 342,
        Rating = 4,
    },
    new HotelInfo
    {
        Name = @"Fairmont Pacific Rim",
        Location = @"Canada",
        AvailableFromDate = new DateOnly(2024, 05, 1),
        AvailableToDate = new DateOnly(2024, 08, 31),
        Price = 444,
        Rating = 5,
    },
};

/* Application Middleware Configuration */

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

    var hotels = hotelsCatalog.SearchHotels(new HotelsSearchFilter()
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
