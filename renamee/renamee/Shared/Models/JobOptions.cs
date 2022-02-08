namespace renamee.Shared.Models
{
    public class JobOptions
    {
        public const string DefaultPattern = "YEAR|YEAR.MONTH.DAY|YEAR.MONTH.DAY-HOUR.MIN.SEC-ORG";

        public string SourceFolder { get; set; } = string.Empty;
        public string DestinationFolder { get; set; } = string.Empty;
        public string FormatPattern { get; set; } = DefaultPattern;
    }
}
