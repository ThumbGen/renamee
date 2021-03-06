using renamee.Server.Repositories;
using renamee.Shared.Models;
using renamee.Shared.Helpers;
using renamee.Shared.Services;

namespace renamee.Server.Services
{
    public interface IJobsService
    {
        Task<IEnumerable<IJob>> GetAll();
        Task Delete(Guid jobId);
        Task AddOrUpdate(IJob job);
        IJob PopulateJob(JobDto jobDto);
        Task Reset(Guid jobId);
    }

    public class JobsService : IJobsService
    {
        private readonly IJobsRepository jobsRepository;
        private readonly JobsFactory jobsFactory;

        public JobsService(IJobsRepository jobsRepository, JobsFactory jobsFactory)
        {
            this.jobsRepository = jobsRepository;
            this.jobsFactory = jobsFactory;
        }

        public async Task AddOrUpdate(IJob job)
        {
            await jobsRepository.AddOrUpdate(job.ToDto());
        }

        public async Task Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
        }

        public async Task<IEnumerable<IJob>> GetAll()
        {
            return (await jobsRepository.GetAll()).Select(x => PopulateJob(x));
        }

        public IJob PopulateJob(JobDto jobDto)
        {
            var job = jobsFactory.Get<IJob>();
            job.AssignFrom(jobDto);
            return job;
        }

        public async Task Reset(Guid jobId)
        {
            var jobDto = await jobsRepository.Get(jobId);
            if(jobDto != null)
            {
                // let the job model decide what reset means
                var job = jobsFactory.Get<IJob>();
                job.AssignFrom(jobDto);
                job.Reset();
                await jobsRepository.AddOrUpdate(job.ToDto());
            }
        }
    }
}
