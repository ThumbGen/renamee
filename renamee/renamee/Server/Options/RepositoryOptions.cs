namespace renamee.Server.Options
{
    public class RepositoryOptions
    {
        public const string Repository = "Repository";

        public string DatabasePath { get; set; } = string.Empty;
        public string DatabasePathLinux { get; set; } = string.Empty;
    }
}
