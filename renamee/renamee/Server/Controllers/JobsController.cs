using Microsoft.AspNetCore.Mvc;
using renamee.Server.Repositories;
using renamee.Shared.DTOs;

namespace renamee.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Put([FromBody]JobDto job)
        {
            if (!ModelState.IsValid)
            { 
                return new StatusCodeResult(412);
            }
            await jobsRepository.AddOrUpdate(job);
            return NoContent();
        }

        [HttpDelete("{jobId:guid}")]
        public async Task<IActionResult> Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]JobDto job)
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