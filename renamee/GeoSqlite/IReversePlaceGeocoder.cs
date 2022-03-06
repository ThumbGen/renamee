namespace ReversePlace
{
    public  interface IReversePlaceGeocoder
    {
        IPlace GetNearestPlace(double latitude, double longitude, string country);
    }
}