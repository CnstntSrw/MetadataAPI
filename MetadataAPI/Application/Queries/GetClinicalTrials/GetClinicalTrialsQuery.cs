using MediatR;
using MetadataAPI.Domain.Entities;

namespace MetadataAPI.Application.Queries.GetClinicalTrials
{
    public record GetClinicalTrialsQuery(string? Status, DateOnly? StartDate, DateOnly? EndDate) : IRequest<List<ClinicalTrialMetadata>>;

}
