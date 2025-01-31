using MediatR;
using MetadataAPI.Application.Interfaces;
using MetadataAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetadataAPI.Application.Queries.GetClinicalTrialById
{
    public class GetClinicalTrialByIdQueryHandler : IRequestHandler<GetClinicalTrialByIdQuery, ClinicalTrialMetadata?>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetClinicalTrialByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClinicalTrialMetadata?> Handle(GetClinicalTrialByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.ClinicalTrialMetadata
                .FirstOrDefaultAsync(t => t.TrialId == request.TrialId, cancellationToken);
        }
    }
}
