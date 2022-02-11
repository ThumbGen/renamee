namespace renamee.Shared.Models
{
    public class JobOptions
    {
        public const string DefaultPattern = "YEAR|YEAR.MONTH.DAY|YEAR.MONTH.DAY-HOUR.MIN.SEC-ORG";
        private string sourceFolder = string.Empty;
        private string destinationFolder = string.Empty;

        public string SourceFolder
        {
            get => sourceFolder;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Path.EndsInDirectorySeparator(value))
                    {
                        value += Path.DirectorySeparatorChar;
                    }
                }
                sourceFolder = value ?? string.Empty;
            }
        }
        public string DestinationFolder
        {
            get => destinationFolder;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Path.EndsInDirectorySeparator(value))
                    {
                        value += Path.DirectorySeparatorChar;
                    }
                }
                destinationFolder = value ?? string.Empty;
            }
        }
        public string FormatPattern { get; set; } = DefaultPattern;
    }
}
