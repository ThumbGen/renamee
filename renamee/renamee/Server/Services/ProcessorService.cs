using renamee.Server.Repositories;
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
            if (!(await jobsRepository.GetAll()).Any())
            {
                var job = serviceProvider.GetRequiredService<Job>();
                job.JobId = Guid.Parse("bb1e5584-fcea-489c-b0b2-8b0fbfa39796");
                job.Name = "demo job";
                job.ActionType = JobActionType.Copy;
                job.Options.SourceFolder = @"C:\renamee_input\";
                job.Options.DestinationFolder = @"C:\renamee_output\";

                await jobsRepository.AddOrUpdate(job);
            }


            logger.LogInformation("Processing jobs...");
            var sw = new Stopwatch();
            sw.Start();
            foreach (var jobEntity in await jobsRepository.GetAll())
            {
                var job = serviceProvider.GetService<Job>();
                if (job != null)
                {
                    job.AssignFrom(jobEntity);

                    try
                    {
                        await job.Run();
                        logger.LogInformation($"\tJob '{job.Name}' done in {sw.Elapsed:hh\\:mm\\:ss}");

                        await jobsRepository.AddOrUpdate(job);
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
