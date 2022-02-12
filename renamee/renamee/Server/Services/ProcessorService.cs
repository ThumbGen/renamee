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
            if(!(await jobsRepository.GetAll()).Any())
            {
                var job = serviceProvider.GetRequiredService<Job>();
                job.Name = "demo job";
                job.Options.SourceFolder = @"C:\renamee_input\"; //@"C:\Users\rvaca\OneDrive\Pictures\";
                job.Options.DestinationFolder = @"C:\renamee_output\";

                await jobsRepository.AddOrUpdate(job);
            }


            logger.LogInformation("Processing jobs...");
            var sw = new Stopwatch();
            sw.Start();
            foreach (var job in await jobsRepository.GetAll())
            {
                try
                {
                    await job.Run();
                    logger.LogInformation($"\tJob '{job.Name}' done in {sw.Elapsed:hh\\:mm\\:ss}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"\tFailed processing job {job.Name} / {job.JobId}", ex);
                }
            }
            sw.Stop();
            logger.LogInformation($"Finish processing jobs in {sw.Elapsed:hh\\:mm\\:ss}");
        }
    }
}
