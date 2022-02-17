using renamee.Server.Repositories;
using renamee.Shared.Interfaces;
using renamee.Shared.Models;

namespace renamee.Server.Services
{
    public class BigDataCloudService : IReverseGeocoder
    {
        private readonly string apiKey = "";
        private readonly string language = "en";
        private readonly HttpClient httpClient;

        private static readonly GeocodingData Empty = new GeocodingData(string.Empty, string.Empty);

        public BigDataCloudService(ISettingsRepository settingsRepository)
        {
            var settings = settingsRepository.Load()?.Geocoder;
            apiKey = settings?.APIKey ?? string.Empty;
            language = settings?.Language ?? "en";

            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public async Task<GeocodingData> Resolve(double latitude, double longitude)
        {
            if (!string.IsNullOrWhiteSpace(apiKey) && !double.IsNaN(latitude) && !double.IsNaN(longitude))
            {
                var uri = $"https://api.bigdatacloud.net/data/reverse-geocode?latitude={Fmt(latitude)}&longitude={Fmt(longitude)}&localityLanguage={language}&key={apiKey}";
                try
                {
                    var data = await httpClient.GetFromJsonAsync<BigDataCloudResponse>(uri);
                    if (data != null && data.status == string.Empty)
                    {
                        return new GeocodingData(string.IsNullOrWhiteSpace(data.locality) ? data.city : data.locality, data.countryName);
                    }
                }
                catch
                {

                }
            }

            return Empty;

            static string Fmt(double val)
            {
                return val.ToString().Replace(',', '.');
            }
        }
    }

    internal class BigDataCloudResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string city { get; set; } = string.Empty;
        public string locality { get; set; } = string.Empty;
        public string countryName { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Naming Styles
    }
}
