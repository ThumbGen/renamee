using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;

//dotnet tool install --global dotnet-ef
//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet ef migrations add InitialCreate
//dotnet ef database update
namespace ReversePlace
{
    public class PlaceGeocoderContext : DbContext
    {
        public DbSet<Place> Locations { get; set; }

        public string DbPath { get; }

        public static string LocationsFile;

        // Sets the file with locations and runs db creation/migration
        public void Initialize(string file)
        {
            Database.Migrate();

            LocationsFile = file;

            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                throw new Exception("Locations file not found.");
            }
            else if (fileInfo.Extension.ToLower() == ".zip")
            {
                LocationsFile = UnZip(file);
            }
        }

        public PlaceGeocoderContext()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DbPath = Path.Join(path, "geo.sqlite");
        }

        public void DeleteDatabase()
        {
            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }
        }

        public void LoadCountryInCache(string countryCode)
        {
            int cont = 0;

            CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
            conf.DetectDelimiter = true;

            using (var reader = new StreamReader(LocationsFile))
            using (var csv = new CsvReader(reader, conf))
            {
                var locations = csv.GetRecords<PlaceCSV>().Where(c => c.CountryCode == countryCode);

                using (var db = new PlaceGeocoderContext())
                {
                    foreach (var location in locations)
                    {
                        Debug.WriteLine(cont + " " + location.Name);
                        cont++;

                        db.Add(PlaceCSV.ToPlace(location));
                    }

                    Debug.WriteLine("saving...");
                    db.SaveChanges();
                    Debug.WriteLine("Cache updated with " + countryCode + " " + db.DbPath);
                }
            }
        }

        private static string UnZip(string file)
        {
            var fileInfo = new FileInfo(file);

            using (var archive = ZipFile.OpenRead(file))
            {
                //since there is only one entry grab the first
                var entry = archive.Entries.First();
                file = entry.FullName;
            }
            if (!File.Exists(file))
            {
                ZipFile.ExtractToDirectory(fileInfo.FullName, fileInfo.Directory.FullName, overwriteFiles: true);
            }

            return file;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
