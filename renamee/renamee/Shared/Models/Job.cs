using Microsoft.Extensions.Logging;
using renamee.Shared.Helpers;

namespace renamee.Shared.Models
{
    public interface IJob
    {
        JobOptions Options { get; set; } 
        Guid JobId { get; }
        string Name { get; set; }
        JobActionType ActionType { get; set; }
        bool IsEnabled { get; set; }
        bool IsRunning { get; }
        DateTimeOffset LastExecutedOn { get; set; }
        DateTimeOffset LastProcessedFileModifiedOn { get; set; }
        void AssignFrom(JobDto job);
        void Reset();
        string GetDemoFormat();
    }
    

    public class Job: IJob
    {
        protected readonly ILogger<Job> logger;

        public JobOptions Options { get; set; } = new JobOptions();

        public Guid JobId { get; internal set; } = Guid.NewGuid();

        public string Name { get; set; } = "unknown";

        public JobActionType ActionType { get; set; } = JobActionType.Simulate;

        public bool IsEnabled { get; set; } = false;

        public bool IsRunning { get; protected set; } = false;

        public DateTimeOffset LastExecutedOn { get; set; } = DateTimeOffset.MinValue;

        public DateTimeOffset LastProcessedFileModifiedOn { get; set; } = DateTimeOffset.MinValue;

        public Job(ILogger<Job> logger)
        {
            this.logger = logger;
        }

        public void AssignFrom(JobDto job)
        {
            JobId = job.JobId;
            Name = job.Name;
            LastExecutedOn = job.LastExecutedOn;
            LastProcessedFileModifiedOn = job.LastProcessedFileModifiedOn;
            ActionType = job.ActionType;
            IsEnabled = job.IsEnabled;
            Options.AssignFrom(job.Options);
        }

        public void Reset()
        {
            LastProcessedFileModifiedOn = DateTimeOffset.MinValue;
        }

        public string GetDemoFormat()
        {
            try
            {
                var date = DateTimeOffset.UtcNow;
                var filename = "MyFile.png";
                var ext = ".png";
                GeocodingData geocodingData = new GeocodingData("MyCity", "MyCountry");

                if (Options != null && FormatParser.TryParse(date, Options.FormatPattern, filename, out string finalSegments, geocodingData))
                {
                    return finalSegments.Replace("|", @"\") + ext;
                }
            }
            catch { }
            return "Invalid pattern";
        }
    }
}
