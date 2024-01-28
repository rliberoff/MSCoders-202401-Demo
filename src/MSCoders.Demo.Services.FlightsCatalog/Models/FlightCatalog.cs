using System.Collections.ObjectModel;

namespace MSCoders.Demo.Services.FlightsCatalog.Models;

internal sealed class FlightCatalog : Collection<FlightInfo>
{
    public IEnumerable<FlightInfo> SearchFlights(FlightsSearchFilter filter)
    {
        return this.Where(flight => (string.IsNullOrWhiteSpace(filter.FromAirport) || flight.FromAirport.Equals(filter.FromAirport, StringComparison.OrdinalIgnoreCase))
                                    && (string.IsNullOrWhiteSpace(filter.ToAirport) || flight.ToAirport.Equals(filter.ToAirport, StringComparison.OrdinalIgnoreCase))
                                    && (filter.FromDate == null || flight.FromDate <= filter.FromDate)
                                    && (filter.ToDate == null || flight.ToDate >= filter.ToDate)
                                    && (filter.MinPrice == null || flight.Price >= filter.MinPrice)
                                    && (filter.MaxPrice == null || flight.Price <= filter.MaxPrice));
    }

    internal static readonly FlightCatalog DemoFlightCatalog =
    [
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
    ];
}
