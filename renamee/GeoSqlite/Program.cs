using ReversePlace;

var db = new PlaceGeocoderContext();
db.Initialize("locations.zip");

// This should automatically trigger the population of AT in cache (db) - slow
var location = new ReversePlaceGeocoder().GetNearestPlace(48.20849, 16.37208, "AT");
Console.WriteLine(location.Name);

// Second query to AT is fast
location = new ReversePlaceGeocoder().GetNearestPlace(47.41427, 9.74195, "AT");
Console.WriteLine(location.Name);

// Put a country in cache
db.LoadCountryInCache("CH");

// This query should be fast
location = new ReversePlaceGeocoder().GetNearestPlace(47.42391, 9.37477, "CH");
Console.WriteLine(location.Name);

Console.ReadLine();
