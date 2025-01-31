using MetadataAPI.Application.Interfaces;
using MetadataAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetadataAPI.Infrastructure.Persistent
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClinicalTrialMetadata> ClinicalTrialMetadata { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClinicalTrialMetadata>()
                .HasKey(x => x.TrialId);

            modelBuilder.Entity<ClinicalTrialMetadata>()
                .HasIndex(e => e.TrialId)
                .IsUnique();
        }
    }
}
