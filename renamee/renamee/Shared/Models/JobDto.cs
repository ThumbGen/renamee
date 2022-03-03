namespace renamee.Shared.Models
{
    public record JobDto()
    {
        public JobOptionsDto Options { get; set; } = new();

        public Guid JobId { get; set; }

        public string Name { get; set; } = string.Empty;

        public JobActionType ActionType { get; set; } = JobActionType.Simulate;

        public bool IsEnabled { get; set; } = false;

        public DateTimeOffset LastExecutedOn { get; set; } = DateTimeOffset.MinValue;

        public DateTimeOffset LastProcessedFileModifiedOn { get; set; } = DateTimeOffset.MinValue;
    }

    public record JobOptionsDto
    {
        public const string DefaultPattern = "YEAR|YEAR.MONTH.DAY|YEAR.MONTH.DAY-HOUR.MIN.SEC-ORG";

        public string SourceFolder { get; set; } = string.Empty;
        public string DestinationFolder { get; set; } = string.Empty;
        public string FormatPattern { get; set; } = DefaultPattern;
    }
}
