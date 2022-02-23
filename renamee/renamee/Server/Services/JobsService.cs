using renamee.Server.Repositories;
using renamee.Shared.Models;
using renamee.Shared.Helpers;

namespace renamee.Server.Services
{
    public interface IJobsService
    {
        Task<IEnumerable<Job>> GetAll();
        Task Delete(Guid jobId);
        Task AddOrUpdate(Job job);
        Job PopulateJob(JobDto jobDto);
    }

    public class JobsService : IJobsService
    {
        private readonly IJobsRepository jobsRepository;
        private readonly IServiceProvider serviceProvider;

        public JobsService(IJobsRepository jobsRepository, IServiceProvider serviceProvider)
        {
            this.jobsRepository = jobsRepository;
            this.serviceProvider = serviceProvider;
        }

        public async Task AddOrUpdate(Job job)
        {
            await jobsRepository.AddOrUpdate(job.ToDto());
        }

        public async Task Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
        }

        public async Task<IEnumerable<Job>> GetAll()
        {
            return (await jobsRepository.GetAll()).Select(x => PopulateJob(x));
        }

        public Job PopulateJob(JobDto jobDto)
        {
            var job = serviceProvider.GetService<Job>();
            job.AssignFrom(jobDto);
            return job;
        }
    }
}
