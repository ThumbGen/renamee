

namespace renamee.Shared.Models
{
    public record Settings
    {
        public GeocoderSettings Geocoder { get; set; } = new GeocoderSettings();
        
    }

    public record GeocoderSettings
    {
        public string APIKey { get; set; } = string.Empty;
        public string Language { get; set; } = "en";
    }
}
