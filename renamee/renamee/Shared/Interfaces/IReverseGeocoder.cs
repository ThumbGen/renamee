using renamee.Shared.Models;

namespace renamee.Shared.Interfaces
{
    public interface IReverseGeocoder
    {
        Task<GeocodingData> Resolve(double latitude, double longitude);
    }
}
