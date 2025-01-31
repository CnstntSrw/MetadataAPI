using MediatR;
using MetadataAPI.Application.Interfaces;
using MetadataAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetadataAPI.Application.Queries.GetClinicalTrials
{
    public class GetClinicalTrialsQueryHandler : IRequestHandler<GetClinicalTrialsQuery, List<ClinicalTrialMetadata>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetClinicalTrialsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ClinicalTrialMetadata>> Handle(GetClinicalTrialsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.ClinicalTrialMetadata.AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(t => t.Status == request.Status);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(t => t.StartDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(t => t.EndDate <= request.EndDate.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
