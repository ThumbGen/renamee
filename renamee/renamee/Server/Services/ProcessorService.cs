using renamee.Server.Repositories;
using renamee.Shared.Helpers;
using renamee.Shared.Models;
using renamee.Shared.Services;
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
        private readonly JobsFactory jobsFactory;

        public ProcessorService(ILogger<ProcessorService> logger, IJobsRepository jobsRepository, JobsFactory jobsFactory)
        {
            this.logger = logger;
            this.jobsRepository = jobsRepository;
            this.jobsFactory = jobsFactory;
        }

        public async Task Process()
        {
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

                var job = jobsFactory.Get<IRunnableJob>();
                if (job != null)
                {
                    job.AssignFrom(jobEntity);

                    try
                    {
                        logger.LogInformation($"\tProcessing job '{jobEntity.Name}'.");
                        await job.Run();
                        logger.LogInformation($"\tJob '{job.Name}' done in {sw.Elapsed:hh\\:mm\\:ss}");
                        // persist state after running
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
