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
}
