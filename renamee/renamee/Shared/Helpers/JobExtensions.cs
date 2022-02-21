using renamee.Shared.Models;

namespace renamee.Shared.Helpers
{
    public static class JobExtensions
    {
        public static JobDto ToDto(this Job job)
        {
            return new JobDto
            {
                ActionType = job.ActionType,
                IsEnabled = job.IsEnabled,
                JobId = job.JobId,
                LastExecutedOn = job.LastExecutedOn,
                Name = job.Name,
                Options = new JobOptionsDto
                {
                    DestinationFolder = job.Options.DestinationFolder,
                    SourceFolder = job.Options.SourceFolder,
                    FormatPattern = job.Options.FormatPattern
                }
            };
        }

        public static void FromDto(this Job job, JobDto dto)
        {
            job.AssignFrom(dto);
        }
    }
}
