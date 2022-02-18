using Microsoft.AspNetCore.Mvc;
using renamee.Server.Repositories;
using renamee.Shared.DTOs;

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
        public async Task<IEnumerable<JobDto>> Get()
        {
            return await jobsRepository.GetAll();
        }

        [HttpPut]
        public async Task<IActionResult> Put(JobDto job)
        {
            if (!ModelState.IsValid)
            { 
                return new StatusCodeResult(412);
            }
            await jobsRepository.AddOrUpdate(job);
            return NoContent();
        }

        [HttpDelete]
        public async Task Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobDto job)
        {
            if (!ModelState.IsValid)
            {
                return new StatusCodeResult(412);
            }
            await jobsRepository.AddOrUpdate(job);
            return NoContent();
        }
    }
}