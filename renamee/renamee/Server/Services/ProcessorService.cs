using renamee.Server.Repositories;
using renamee.Shared.DTOs;
using renamee.Shared.Helpers;
using renamee.Shared.Models;
using System.Diagnostics;

namespace renamee.Server.Services
{
    public interface IProcessorService
    {
        Task Process();
    }

    public class ProcessorService : IProcessorService
    {
        private readonly ILogger<ProcessorService> logger;
        private readonly IJobsRepository jobsRepository;
        private readonly IServiceProvider serviceProvider;

        public ProcessorService(ILogger<ProcessorService> logger, IJobsRepository jobsRepository, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.jobsRepository = jobsRepository;
            this.serviceProvider = serviceProvider;
        }

        public async Task Process()
        {
            // TODO remove
            if (!(await jobsRepository.GetAll()).Any())
            {
                var job = new JobDto
                {
                    JobId = Guid.Parse("bb1e5584-fcea-489c-b0b2-8b0fbfa39796"),
                    Name = "demo job",
                    ActionType = JobActionType.Copy,
                    Options = new JobOptionsDto
                    {
                        SourceFolder = @"C:\renamee_input\",
                        DestinationFolder = @"C:\renamee_output\"
                    }
                };
                job.Options.SourceFolder = @"C:\renamee_input\";
                job.Options.DestinationFolder = @"C:\renamee_output\";

                await jobsRepository.AddOrUpdate(job);
            }


            logger.LogInformation("Processing jobs...");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var jobEntity in await jobsRepository.GetAll())
            {
                if (!jobEntity.IsEnabled)
                {
                    logger.LogInformation($"\tSkipping job '{jobEntity.Name}' as it is disabled.");
                    continue;
                }

                var job = serviceProvider.GetService<Job>();
                if (job != null)
                {
                    job.AssignFrom(jobEntity);

                    try
                    {
                        await job.Run();
                        logger.LogInformation($"\tJob '{job.Name}' done in {sw.Elapsed:hh\\:mm\\:ss}");

                        await jobsRepository.AddOrUpdate(job.ToDto());
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"\tFailed processing job {job.Name} / {job.JobId}", ex);
                    }
                }
                else logger.LogError("Can't resolve a Job");
            }

            sw.Stop();
            logger.LogInformation($"Finish processing jobs in {sw.Elapsed:hh\\:mm\\:ss}");
        }
    }
}
