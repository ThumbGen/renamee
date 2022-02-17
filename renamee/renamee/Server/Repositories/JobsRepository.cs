using JsonFlatFileDataStore;
using Microsoft.Extensions.Options;
using renamee.Server.Options;
using renamee.Shared.Models;

namespace renamee.Server.Repositories
{
    public interface IJobsRepository
    {
        Task<IEnumerable<Job>> GetAll();
        Task Delete(Guid jobId);
        Task AddOrUpdate(Job job);
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

        public async Task AddOrUpdate(Job job)
        {
            var jobs = store.GetCollection<Job>();
            await DeleteCore(jobs, job.JobId);
            await jobs.InsertOneAsync(job);
        }

        public async Task Delete(Guid jobId)
        {
            var jobs = store.GetCollection<Job>();
            await DeleteCore(jobs, jobId);
        }

        public Task<IEnumerable<Job>> GetAll()
        {
            return Task.FromResult(store.GetCollection<Job>().AsQueryable().AsEnumerable());
        }

        private static async Task DeleteCore(IDocumentCollection<Job> jobs, Guid jobId)
        {
            var found = Find(jobs, jobId);
            if (found != null)
            {
                await jobs.DeleteOneAsync(x => x.JobId == found.JobId);
            }
        }

        private static Job? Find(IDocumentCollection<Job> jobs, Guid jobId)
        {
            return jobs.Find(x => x.JobId == jobId).SingleOrDefault();
        }
    }
}
