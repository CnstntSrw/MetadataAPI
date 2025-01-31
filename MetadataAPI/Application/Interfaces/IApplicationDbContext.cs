using MetadataAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetadataAPI.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ClinicalTrialMetadata> ClinicalTrialMetadata { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
