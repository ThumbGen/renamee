using renamee.Shared.Models;

namespace renamee.Shared.Helpers
{
    public static class JobExtensions
    {
        public static JobDto ToDto(this IJob job)
        {
            return new JobDto
            {
                ActionType = job.ActionType,
                IsEnabled = job.IsEnabled,
                IsRunning = job.IsRunning,
                JobId = job.JobId,
                LastExecutedOn = job.LastExecutedOn,
                LastProcessedFileModifiedOn = job.LastProcessedFileModifiedOn,
                Name = job.Name,
                Options = new JobOptionsDto
                {
                    DestinationFolder = job.Options.DestinationFolder,
                    SourceFolder = job.Options.SourceFolder,
                    FormatPattern = job.Options.FormatPattern
                }
            };
        }

        public static void FromDto(this IJob job, JobDto dto)
        {
            job.AssignFrom(dto);
        }
    }
}
