using MediatR;
using MetadataAPI.Application.Commands.UploadJSONMetadata;
using MetadataAPI.Application.Queries.GetClinicalTrialById;
using MetadataAPI.Application.Queries.GetClinicalTrials;
using Microsoft.AspNetCore.Mvc;

namespace MetadataAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ClinicalTrialController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Retrieves a clinical trial record by trialId
        /// </summary>
        /// <param name="trialId">The unique identifier of the clinical trial.</param>
        /// <returns>The clinical trial details.</returns>
        [HttpGet("{trialId}")]
        public async Task<IActionResult> GetById(string trialId)
        {
            var result = await _mediator.Send(new GetClinicalTrialByIdQuery(trialId));
            if (result == null) return NotFound($"Trial with ID {trialId} not found.");

            return Ok(result);
        }
        /// <summary>
        /// Retrieves clinical trials with applied filter
        /// </summary>
        /// <param name="status">Clinical trial status</param>
        /// <param name="startDate">Clinical trial start date</param>
        /// <param name="endDate">Clinical trial end date</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrials([FromQuery] string? status, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            var query = new GetClinicalTrialsQuery(status, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        /// <summary>
        /// Upload the clinical trial metadata.
        /// Constraints:
        /// - must be .json file
        /// - size of the file must be lower than 10 Mb
        /// </summary>
        /// <param name="file">JSON file</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadJsonMetadata(IFormFile file)
        {
            var result = await _mediator.Send(new UploadJsonMetadataCommand(file));

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok("File uploaded and processed successfully.");
        }
    }
}
