using Microsoft.AspNetCore.Mvc;
using renamee.Server.Repositories;
using renamee.Shared.Models;

namespace renamee.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsRepository jobsRepository;

        public JobsController(IJobsRepository jobsRepository)
        {
            this.jobsRepository = jobsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Job>> Get()
        {
            return await jobsRepository.GetAll();
        }

        [HttpPut]
        public async Task Put(Job job)
        {
            await jobsRepository.AddOrUpdate(job);
        }

        [HttpDelete]
        public async Task Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
        }

        [HttpPost]
        public async Task Create(Job job)
        {
            await jobsRepository.AddOrUpdate(job);
        }
    }
}