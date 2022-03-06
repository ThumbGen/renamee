using Geolocation;
using System.Diagnostics;

namespace ReversePlace
{
    public class ReversePlaceGeocoder : IReversePlaceGeocoder
    {
        public IPlace GetNearestPlace(double latitude, double longitude, string countryCode)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            Place? nearestLocation = null;

            var origin = new Coordinate(latitude, longitude);
            var maxDistance = 0.1;

            using (var db = new PlaceGeocoderContext())
            {
                // If country missing in cache, load from CSV file
                if (!db.Locations.Any(l=>l.CountryCode == countryCode))
                {
                    db.LoadCountryInCache(countryCode);
                }

                while (nearestLocation == null && maxDistance < 100) 
                {
                    var boundaries = new CoordinateBoundaries(origin, distance: maxDistance, DistanceUnit.Kilometers);

                    var nearlocations = db.Locations.Where(x =>
                        x.Latitude >= boundaries.MinLatitude && x.Latitude <= boundaries.MaxLatitude && // within latitude range
                        x.Longitude >= boundaries.MinLongitude && x.Longitude <= boundaries.MaxLongitude)  // within longitude range
                        .ToList() // force query, ¨GetDistance¨needs to be executed by client, not as custom function
                        .Select(loc => new
                        {
                            Location = loc,
                            Distance = GeoCalculator.GetDistance(origin.Latitude, origin.Longitude, loc.Latitude, loc.Longitude, 4, DistanceUnit.Kilometers)
                        })
                        .OrderBy(x => x.Distance); // First element will be the closet location


                    nearestLocation = nearlocations.FirstOrDefault()?.Location;

                    if (nearestLocation == null)
                    {
                        maxDistance = maxDistance * 2;
                        Debug.WriteLine("Expand range to a square of " + maxDistance + " km.");
                    }
                };
            }

            time.Stop();
            Console.WriteLine(time.Elapsed);

            return nearestLocation;
        }
    }
}
