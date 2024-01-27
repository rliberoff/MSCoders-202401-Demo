using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.Extensions.Logging.ApplicationInsights;

using MSCoders.Demo.Services.Common;
using MSCoders.Demo.Services.FunCatalog.Models;

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

var funCatalog = new FunCatalog()
{
    // Cancun
    new FunInfo
    {
        Name = @"Dolphin Sightseeing ",
        Location = @"Cancun",
        Price = 40,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"scuba diving",
        Location = @"Cancun",
        Price = 30,
        Rating = 4,
    },
    new FunInfo
    {
        Name = @"Chichen Itza, Cenote and Valladolid All-Inclusive Tour",
        Location = @"Cancun",
        Price = 70,
        Rating = 5,
    },
    new FunInfo
    {
        Name = @"ATV Jungle Adventure with Ziplines, Cenote & Tequila Tasting",
        Location = @"Cancun",
        Price = 70,
        Rating = 2,
    },

    // Dominican Rep.
    new FunInfo
    {
        Name = @"Half-Day Buggy Tour",
        Location = @"Dominican Republic",
        Price = 54,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"Small-Group Cruising",
        Location = @"Dominican Republic",
        Price = 100,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"Saona Island Day Trip",
        Location = @"Dominican Republic",
        Price = 178,
        Rating = 4,
    },
    new FunInfo
    {
        Name = @"Renaissance Santo Domingo Jaragua",
        Location = @"República Dominicana",
        Price = 210,
        Rating = 4,
    },
    new FunInfo
    {
        Name = @"Damajagua The 7 waterfalls excursion ",
        Location = @"Dominican Republic",
        Price = 55,
        Rating = 3,
    },

    // Punta Cana
    new FunInfo
    {
        Name = @"Scape Park Full Day Punta Cana",
        Location = @"Punta Cana",
        Price = 130,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"Horseback Riding",
        Location = @"Punta Cana",
        Price = 58,
        Rating = 5,
    },
    new FunInfo
    {
        Name = @"Cocobongo",
        Location = @"Punta Cana",
        Price = 85,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"Monkeyland and Plantation Safari",
        Location = @"Punta Cana",
        Price = 95,
        Rating = 4,
    },   

    // Bahamas
    new FunInfo
    {
        Name = @"4-Hour Tour in Bahamas with Jet Ski",
        Location = @"Bahamas",
        Price = 450,
        Rating = 4,
    },
    new FunInfo
    {
        Name = @"Exuma Island Hopping & Swimming Pigs Tour",
        Location = @"Bahamas",
        Price = 439,
        Rating = 2,
    },
    new FunInfo
    {
        Name = @"Pirate Jeep Tours Sightseeing Adventure!",
        Location = @"Bahamas",
        Price = 242,
        Rating = 3,
    },

    // Switzerland
    new FunInfo
    {
        Name = @"Zurich Sightseeing With Lake Cruise and Lindt Home of Chocolate",
        Location = @"Switzerland",
        Price = 90,
        Rating = 3,
    },
    new FunInfo
    {
        Name = @"Verkehrshaus der Schweiz",
        Location = @"Switzerland",
        Price = 21,
        Rating = 2,
    },
    new FunInfo
    {
        Name = @"Pilatus Luzern",
        Location = @"Switzerland",
        Price = 89,
        Rating = 4,
    },

    // Munich
    new FunInfo
    {
        Name = @"Dachau Concentration Camp Memorial Site Tour",
        Location = @"Munich",
        Price = 52,
        Rating = 2,
    },
    new FunInfo
    {
        Name = @"Munich Ghosts and Spirits Evening Walking Tour",
        Location = @"Munich",
        Price = 54,
        Rating = 2,
    },
    new FunInfo
    {
        Name = @"Bavarian Beer and Food Evening Tour in Munich",
        Location = @"Munich",
        Price = 69,
        Rating = 5,
    },

    // Canada
    new FunInfo
    {
        Name = @"Niagara Falls Day Tour from Toronto",
        Location = @"Canada",
        Price = 75,
        Rating = 4,
    },
    new FunInfo
    {
        Name = @"Gastown Historic Walking Food Tour",
        Location = @"Canada",
        Price = 98,
        Rating = 2,
    },
    new FunInfo
    {
        Name = @"Grape to Glass Wine Experience",
        Location = @"Canada",
        Price = 24,
        Rating = 3,
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

app.MapGet(@"/catalog/fun", Results<Ok<IEnumerable<FunInfo>>, NotFound<NotFoundMessage>> (
    string? location = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    [Range(0, 5)] int minRating = 0,
    [Range(0, 5)] int maxRating = 5)
    =>
{
    var hotels = funCatalog.SearchFun(new FunSearchFilter()
    {
        Location = location,
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
