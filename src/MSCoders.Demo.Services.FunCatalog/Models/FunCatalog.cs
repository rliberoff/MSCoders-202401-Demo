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

    internal static readonly FunCatalog DemoFunCatalog =
    [
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
    ];
}
