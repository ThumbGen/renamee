namespace ReversePlace
{
    public class Place : IPlace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CountryCode { get; set; }
    }

    public class PlaceCSV
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CountryCode { get; set; }

        public static Place ToPlace(PlaceCSV loc)
        {
            return new Place { Name = loc.Name, Latitude = loc.Latitude, Longitude = loc.Longitude, CountryCode = loc.CountryCode };
        }
    }
}
