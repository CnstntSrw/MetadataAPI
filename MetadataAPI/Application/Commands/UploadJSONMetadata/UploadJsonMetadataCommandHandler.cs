using MediatR;
using MetadataAPI.Application.Interfaces;
using MetadataAPI.Common;
using MetadataAPI.Domain.Entities;
using MetadataAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MetadataAPI.Application.Commands.UploadJSONMetadata
{
    public class UploadJsonMetadataCommandHandler : IRequestHandler<UploadJsonMetadataCommand, CommonResult>
    {
        private const int MaxFileSizeInBytes = 10 * 1024 * 1024;

        private readonly IApplicationDbContext _dbContext;
        private readonly IJsonSchemaValidator _jsonSchemaValidator;
        private readonly ILogger<UploadJsonMetadataCommandHandler> _logger;

        public UploadJsonMetadataCommandHandler(IApplicationDbContext dbContext,
                                                IJsonSchemaValidator jsonSchemaValidator,
                                                ILogger<UploadJsonMetadataCommandHandler> logger)
        {
            _dbContext = dbContext;
            _jsonSchemaValidator = jsonSchemaValidator;
            _logger = logger;
        }
        public async Task<CommonResult> Handle(UploadJsonMetadataCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return CommonResult.Failure("No file uploaded.");

            if (!request.File.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return CommonResult.Failure("Invalid file format. Only .json files are allowed.");

            if (request.File.Length > MaxFileSizeInBytes)
                return CommonResult.Failure("The uploaded file exceeds the maximum allowed size of 10MB.");


            try
            {
                using var stream = new StreamReader(request.File.OpenReadStream());
                var jsonContent = await stream.ReadToEndAsync();

                // Validate JSON against schema
                var validationResult = await _jsonSchemaValidator.ValidateJsonAsync(jsonContent);
                if (!validationResult.IsValid)
                    return CommonResult.Failure("Invalid JSON format: " + string.Join("; ", validationResult.Errors));

                // Deserialize JSON
                var clinicalTrial = JsonConvert.DeserializeObject<ClinicalTrialMetadata>(jsonContent);

                JsonConvert.DeserializeObject<ClinicalTrialMetadata>(jsonContent);

                ApplyBusinessRules(clinicalTrial);

                // Save to database
                await _dbContext.ClinicalTrialMetadata.AddAsync(clinicalTrial, cancellationToken);
                try
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Error saving clinical trial metadata due to duplicate TrialId.");
                    return CommonResult.Failure(
                        $"A clinical trial with TrialId {clinicalTrial.TrialId} already exists.");
                }

                _logger.LogInformation("Clinical trial metadata uploaded successfully: {TrialId}",
                    clinicalTrial.TrialId);

                return CommonResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing uploaded JSON file.");
                return CommonResult.Failure($"Error processing uploaded JSON file. {ex.Message}");
            }
        }

        private static void ApplyBusinessRules(ClinicalTrialMetadata? clinicalTrial)
        {
            // Business Rule: Default endDate
            if (clinicalTrial.Status == "Ongoing" && clinicalTrial.EndDate == null)
            {
                clinicalTrial.EndDate = clinicalTrial.StartDate.AddMonths(1);
            }

            // Business Rule: Calculate Duration
            clinicalTrial.DurationInDays = clinicalTrial.EndDate.HasValue
                ? (clinicalTrial.EndDate.Value.ToDateTime(TimeOnly.MinValue) -
                   clinicalTrial.StartDate.ToDateTime(TimeOnly.MinValue)).Days
                : (DateTime.Now.Date - clinicalTrial.StartDate.ToDateTime(TimeOnly.MinValue)).Days;
        }
    }
}
