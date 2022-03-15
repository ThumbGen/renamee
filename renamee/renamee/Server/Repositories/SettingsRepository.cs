using JsonFlatFileDataStore;
using Microsoft.Extensions.Options;
using renamee.Server.Options;
using renamee.Shared.Models;

namespace renamee.Server.Repositories
{
    public interface ISettingsRepository
    {
        Settings Load();
        void Save(Settings settings);
    }

    public class SettingsRepository : ISettingsRepository
    {
        const string SettingsKey = "settings";
        private readonly DataStore store;
        private readonly RepositoryOptions options;

        public SettingsRepository(IOptions<RepositoryOptions> repositoryOptions)
        {
            options = repositoryOptions.Value;
            // Open database (create new if file doesn't exist)
            var dbPath = OperatingSystem.IsLinux() ? options.DatabasePathLinux : options.DatabasePath;
            Directory.CreateDirectory(dbPath);
            store = new DataStore(Path.Combine(Path.GetDirectoryName(dbPath), "settings_db.json"));
        }

        public Settings Load()
        {
            try
            {
                return store.GetItem<Settings>(SettingsKey);
            }
            catch
            {
                var s = new Settings();
                Save(s);
                return s;
            }
        }

        public void Save(Settings settings)
        {
            try
            {
                store.ReplaceItem(SettingsKey, settings, true);
            }
            catch 
            {
                //
            }
        }
    }
}
