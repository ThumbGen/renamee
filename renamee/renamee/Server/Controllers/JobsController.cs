using Microsoft.AspNetCore.Mvc;
using renamee.Server.Services;
using renamee.Shared.Helpers;
using renamee.Shared.Models;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet(Name = "GetJobs")]
        [SwaggerOperation(
            Summary = "Get all jobs",
            OperationId = "GetJobs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<JobDto>> Get()
        {
            return (await jobsService.GetAll()).Select(x => x.ToDto());
        }

        [HttpPut(Name = "UpsertJob")]
        [SwaggerOperation(
            Summary = "Upsert the specified job",
            OperationId = "UpsertJob")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Put([FromBody] JobDto job)
        {
            if (!ModelState.IsValid)
            {
                return new StatusCodeResult(412);
            }
            await jobsService.AddOrUpdate(jobsService.PopulateJob(job));
            return NoContent();
        }

        [HttpDelete("{jobId:guid}", Name = "DeleteJob")]
        [SwaggerOperation(
            Summary = "Delete the specified job",
            OperationId = "DeleteJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid jobId)
        {
            await jobsService.Delete(jobId);
            return Ok();
        }

        [HttpPost(Name = "CreateJob")]
        [SwaggerOperation(
            Summary = "Create a new job",
            OperationId = "CreateJob")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Create([FromBody] JobDto job)
        {
            if (!ModelState.IsValid)
            {
                return new StatusCodeResult(412);
            }
            await jobsService.AddOrUpdate(jobsService.PopulateJob(job));
            return NoContent();
        }

        [HttpPost("{jobId:guid}", Name = "ResetJob")]
        [SwaggerOperation(
            Summary = "Reset the specified job",
            OperationId = "ResetJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Reset(Guid jobId)
        {
            await jobsService.Reset(jobId);
            return Ok();
        }
    }
}