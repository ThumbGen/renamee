using renamee.Shared.Interfaces;
using renamee.Shared.Models;

namespace renamee.Server.Services
{
    public class GeoNamesService : IReverseGeocoder
    {
        private const string citiesDb = "cities500.txt";
        //private const string citiesDb = "allcountries.txt";
        private const string countryInfoDb = "countryInfo.txt";
        private readonly string citiesDbPath;
        private readonly string countryInfoDbPath;

        private readonly ReverseGeocoding.Interface.IReverseGeocoder geocoder;


        public GeoNamesService()
        {
            citiesDbPath = citiesDb;
            countryInfoDbPath = countryInfoDb;
            geocoder = new ReverseGeocoding.ReverseGeocoder(citiesDbPath, countryInfoDbPath);
        }

        public Task<GeocodingData> Resolve(double latitude, double longitude)
        {
            if (!double.IsNaN(latitude) && !double.IsNaN(longitude))
            {
                var place = geocoder.GetNearestPlace(latitude, longitude);
                if (place != null)
                {
                    return Task.FromResult(new GeocodingData(place.Name ?? string.Empty, place.CountryInfo?.Country ?? place.CountryCode ?? string.Empty));
                }
            }
            // fallback
            return Task.FromResult(new GeocodingData(string.Empty, string.Empty));
        }
    }
}
