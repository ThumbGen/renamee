namespace ReversePlace
{
    public interface IPlace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CountryCode { get; set; }
    }
}