using renamee.Shared.Helpers;
using renamee.Shared.Models;

namespace renamee.Client.Helpers
{
    public static class JobHelpers
    {
        public static string DemoFormat(this JobDto job)
        {
            try
            {
                var date = DateTimeOffset.UtcNow;
                var filename = "MyFile.png";
                var ext = ".png";
                GeocodingData geocodingData = new GeocodingData("MyCity", "MyCountry");

                if (job?.Options != null && FormatParser.TryParse(date, job.Options.FormatPattern, filename, out string finalSegments, geocodingData))
                {
                    return finalSegments.Replace("|", @"\") + ext;
                }
            }
            catch { }
            return "Invalid pattern";
        }
    }
}
