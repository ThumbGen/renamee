using JsonFlatFileDataStore;
using renamee.Shared.Models;

namespace renamee.Server.Repositories
{
    public interface IJobsRepository
    {
        Task<IEnumerable<IJob>> GetAll();
        Task Delete(IJob job);
        Task AddOrUpdate(IJob job);
    }

    public class JobsRepository : IJobsRepository
    {
        private readonly DataStore store;

        public JobsRepository()
        {
            // Open database (create new if file doesn't exist)
            store = new DataStore("jobs_db.json");
        }

        public async Task AddOrUpdate(IJob job)
        {
            var jobs = store.GetCollection<IJob>();
            var found = Find(jobs, job);
            if (found != null)
            {
                await jobs.DeleteOneAsync(found);
            }
            await jobs.InsertOneAsync(job);
        }

        public async Task Delete(IJob job)
        {
            var jobs = store.GetCollection<IJob>();
            var found = Find(jobs, job);
            if (found != null)
            {
                await jobs.DeleteOneAsync(found);
            }
        }

        public Task<IEnumerable<IJob>> GetAll()
        {
            return Task.FromResult(store.GetCollection<IJob>().AsQueryable().AsEnumerable());
        }

        private static IJob? Find(IDocumentCollection<IJob> jobs, IJob job)
        {
            return jobs.Find(x => x.Id == job.Id).SingleOrDefault();
        }
    }
}
