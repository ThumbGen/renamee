using Microsoft.AspNetCore.Mvc;
using renamee.Server.Services;
using renamee.Shared.Helpers;
using renamee.Shared.Models;

namespace renamee.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService jobsService;

        public JobsController(IJobsService jobsService)
        {
            this.jobsService = jobsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<JobDto>> Get()
        {
            return (await jobsService.GetAll()).Select(x => x.ToDto());
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
            await jobsService.AddOrUpdate(jobsService.PopulateJob(job));
            return NoContent();
        }

        [HttpDelete("{jobId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid jobId)
        {
            await jobsService.Delete(jobId);
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
            await jobsService.AddOrUpdate(jobsService.PopulateJob(job));
            return NoContent();
        }

       
    }
}