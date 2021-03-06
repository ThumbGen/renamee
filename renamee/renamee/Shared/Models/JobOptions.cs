using renamee.Shared.Helpers;

namespace renamee.Shared.Models
{
    public class JobOptions
    {
        private string sourceFolder = string.Empty;
        private string destinationFolder = string.Empty;

        public string SourceFolder
        {
            get => sourceFolder;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.EnsureDirectoryEnding();
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
                    value = value.EnsureDirectoryEnding();
                }
                destinationFolder = value ?? string.Empty;
            }
        }
        public string FormatPattern { get; set; } = JobOptionsDto.DefaultPattern;

        public void AssignFrom(JobOptionsDto jobOptions)
        {
            if (jobOptions != null)
            {
                SourceFolder = jobOptions.SourceFolder;
                DestinationFolder = jobOptions.DestinationFolder;
                FormatPattern = jobOptions.FormatPattern;
            }
        }
    }
}
