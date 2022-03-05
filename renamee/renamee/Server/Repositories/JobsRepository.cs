using JsonFlatFileDataStore;
using Microsoft.Extensions.Options;
using renamee.Server.Options;
using renamee.Shared.Models;

namespace renamee.Server.Repositories
{
    public interface IJobsRepository
    {
        Task<IEnumerable<JobDto>> GetAll();
        Task Delete(Guid jobId);
        Task AddOrUpdate(JobDto job);
        Task<JobDto> Get(Guid jobId);
    }

    public class JobsRepository : IJobsRepository
    {
        private readonly DataStore store;
        private readonly RepositoryOptions options;

        public JobsRepository(IOptions<RepositoryOptions> repositoryOptions)
        {
            options = repositoryOptions.Value;

            // Open database (create new if file doesn't exist)
            store = new DataStore(OperatingSystem.IsLinux() ? options.DatabasePathLinux : options.DatabasePath);
        }

        public async Task AddOrUpdate(JobDto job)
        {
            var jobs = store.GetCollection<JobDto>();
            await DeleteCore(jobs, job.JobId);
            await jobs.InsertOneAsync(job);
        }

        public async Task Delete(Guid jobId)
        {
            var jobs = store.GetCollection<JobDto>();
            await DeleteCore(jobs, jobId);
        }

        public Task<JobDto> Get(Guid jobId)
        {
            return Task.FromResult(Find(store.GetCollection<JobDto>(), jobId));
        }

        public Task<IEnumerable<JobDto>> GetAll()
        {
            return Task.FromResult(store.GetCollection<JobDto>().AsQueryable().AsEnumerable());
        }

        private static async Task DeleteCore(IDocumentCollection<JobDto> jobs, Guid jobId)
        {
            var found = Find(jobs, jobId);
            if (found != null)
            {
                await jobs.DeleteOneAsync(x => x.JobId == found.JobId);
            }
        }

        private static JobDto Find(IDocumentCollection<JobDto> jobs, Guid jobId)
        {
            return jobs.Find(x => x.JobId == jobId).SingleOrDefault();
        }
    }
}
