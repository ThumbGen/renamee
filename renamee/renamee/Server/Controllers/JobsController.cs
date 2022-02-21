using Microsoft.AspNetCore.Mvc;
using renamee.Server.Repositories;
using renamee.Shared.Models;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<JobDto>> Get()
        {
            return await jobsRepository.GetAll();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid jobId)
        {
            await jobsRepository.Delete(jobId);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
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