using MediatR;
using MetadataAPI.Domain.Entities;

namespace MetadataAPI.Application.Queries.GetClinicalTrialById
{
    public record GetClinicalTrialByIdQuery(string TrialId) : IRequest<ClinicalTrialMetadata?>;
}
