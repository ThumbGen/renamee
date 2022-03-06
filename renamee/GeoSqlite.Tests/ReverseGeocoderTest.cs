using NUnit.Framework;

namespace ReversePlace.Tests
{
    [TestFixture]
    public class ReverseGeocoderTest
    {
        private const string testCity1 = "Chumikan";
        private const double testPlaceLatitude1 = 57.2980;
        private const double testPlaceLongitude1 = 134.882;

        private const string testCity2 = "Fuengirola";
        private const double testPlaceLatitude2 = 36.53998;
        private const double testPlaceLongitude2 = -4.62473;
        
        private const string testCity3 = "Bioparc Fuengirola";
        private const double testPlaceLatitude3 = 36.5375;
        private const double testPlaceLongitude3 = -4.6272;
        
        private const string testCity4 = "Vienna";
        private const double testPlaceLatitude4 = 48.20849;
        private const double testPlaceLongitude4 = 16.37208;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            new PlaceGeocoderContext().DeleteDatabase();
            new PlaceGeocoderContext().Initialize("tests.csv");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            new PlaceGeocoderContext().DeleteDatabase();
        }

        [TestCase(testPlaceLatitude1, testPlaceLongitude1, testCity1, "RU")]
        [TestCase(testPlaceLatitude2, testPlaceLongitude2, testCity2, "ES")]
        [TestCase(testPlaceLatitude3, testPlaceLongitude3, testCity3, "ES")]
        [TestCase(testPlaceLatitude4, testPlaceLongitude4, testCity4, "AT")]
        public void ReverseGeocoderNameTests(double latitude, double longitude, string placeName, string country)
        {
            var location = new ReversePlaceGeocoder().GetNearestPlace(latitude, longitude, country); 
            Assert.True(placeName == location.Name);
        }

        [TestCase(testPlaceLatitude2, testPlaceLongitude2, "ES")]
        [TestCase(testPlaceLatitude3, testPlaceLongitude3, "ES")]
        public void ReverseGeocoderCountryTests(double latitude, double longitude, string country)
        {
            var location = new ReversePlaceGeocoder().GetNearestPlace(latitude, longitude, country);
            Assert.True(country == location.CountryCode);
        }
    }
}