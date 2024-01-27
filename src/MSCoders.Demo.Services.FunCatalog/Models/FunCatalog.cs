using System.Collections.ObjectModel;

namespace MSCoders.Demo.Services.FunCatalog.Models;

internal sealed class FunCatalog : Collection<FunInfo>
{
    public IEnumerable<FunInfo> SearchFun(FunSearchFilter filter)
    {
        return this.Where(fun => (string.IsNullOrWhiteSpace(filter.Location) || fun.Location.Equals(filter.Location, StringComparison.OrdinalIgnoreCase))
                                    && (filter.MinPrice == null || fun.Price >= filter.MinPrice)
                                    && (filter.MaxPrice == null || fun.Price <= filter.MaxPrice)
                                    && (fun.Rating >= filter.MinRating)
                                    && (fun.Rating <= filter.MaxRating));
    }
}
