using System.Collections.ObjectModel;

namespace MSCoders.Demo.Services.HotelsCatalog.Models;

internal sealed class HotelsCatalog : Collection<HotelInfo>
{
    public IEnumerable<HotelInfo> SearchHotels(HotelsSearchFilter filter)
    {
        return this.Where(hotel => (string.IsNullOrWhiteSpace(filter.Location) || hotel.Location.Equals(filter.Location, StringComparison.OrdinalIgnoreCase))
                                    && (filter.FromDate == null || hotel.AvailableFromDate <= filter.FromDate)
                                    && (filter.ToDate == null || hotel.AvailableToDate >= filter.ToDate)
                                    && (filter.MinPrice == null || hotel.Price >= filter.MinPrice)
                                    && (filter.MaxPrice == null || hotel.Price <= filter.MaxPrice)
                                    && (hotel.Rating >= filter.MinRating)
                                    && (hotel.Rating <= filter.MaxRating));
    }
}
