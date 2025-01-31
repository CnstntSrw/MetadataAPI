using MediatR;
using MetadataAPI.Application.Commands.UploadJSONMetadata;
using MetadataAPI.Application.Queries.GetClinicalTrialById;
using MetadataAPI.Application.Queries.GetClinicalTrials;
using MetadataAPI.Infrastructure.Interfaces;
using MetadataAPI.Infrastructure.Persistent;
using Microsoft.AspNetCore.Mvc;

namespace MetadataAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialController : ControllerBase
    {
        private readonly IJsonSchemaValidator _jsonSchemaValidator;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ClinicalTrialController> _logger;
        private readonly IMediator _mediator;
        public ClinicalTrialController(IMediator mediator, IJsonSchemaValidator jsonSchemaValidator, ApplicationDbContext dbContext, ILogger<ClinicalTrialController> logger)
        {
            _jsonSchemaValidator = jsonSchemaValidator;
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet("{trialId}")]
        public async Task<IActionResult> GetById(string trialId)
        {
            var result = await _mediator.Send(new GetClinicalTrialByIdQuery(trialId));
            if (result == null) return NotFound($"Trial with ID {trialId} not found.");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrials([FromQuery] string? status, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            var query = new GetClinicalTrialsQuery(status, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
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
