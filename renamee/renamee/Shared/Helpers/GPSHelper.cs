using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace renamee.Shared.Helpers
{
    public static class GPSHelper
    {
        public static (double latitude, double longitude) GetCoordinates(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var gpsDirectory = directories.OfType<GpsDirectory>().FirstOrDefault();
            if (gpsDirectory != null)
            {
                var geoLocation = gpsDirectory.GetGeoLocation();
                if(geoLocation != null)
                {
                    return (geoLocation.Latitude, geoLocation.Longitude);
                }
            }
            return (double.NaN, double.NaN);
        }
    }
}
