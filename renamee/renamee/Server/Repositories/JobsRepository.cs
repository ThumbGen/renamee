using JsonFlatFileDataStore;
using Microsoft.Extensions.Options;
using renamee.Server.Options;
using renamee.Shared.Models;

namespace renamee.Server.Repositories
{
    public interface IJobsRepository
    {
        Task<IEnumerable<Job>> GetAll();
        Task Delete(Job job);
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
            store = new DataStore(Path.Combine(OperatingSystem.IsLinux() ? options.DatabasePathLinux : options.DatabasePath, "jobs_db.json"));
        }

        public async Task AddOrUpdate(Job job)
        {
            var jobs = store.GetCollection<Job>();
            var found = Find(jobs, job);
            if (found != null)
            {
                await jobs.DeleteOneAsync(found);
            }
            await jobs.InsertOneAsync(job);
        }

        public async Task Delete(Job job)
        {
            var jobs = store.GetCollection<Job>();
            var found = Find(jobs, job);
            if (found != null)
            {
                await jobs.DeleteOneAsync(found);
            }
        }

        public Task<IEnumerable<Job>> GetAll()
        {
            return Task.FromResult(store.GetCollection<Job>().AsQueryable().AsEnumerable());
        }

        private static Job? Find(IDocumentCollection<Job> jobs, Job job)
        {
            return jobs.Find(x => x.JobId == job.JobId).SingleOrDefault();
        }
    }
}
