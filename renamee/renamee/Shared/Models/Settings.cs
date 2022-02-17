

namespace renamee.Shared.Models
{
    public class Settings
    {
        public GeocoderSettings Geocoder { get; set; } = new GeocoderSettings();
        
    }

    public class GeocoderSettings
    {
        public string APIKey { get; set; } = string.Empty;
        public string Language { get; set; } = "en";
    }
}
