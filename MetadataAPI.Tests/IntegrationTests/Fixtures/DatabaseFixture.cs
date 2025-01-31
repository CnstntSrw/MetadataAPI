using MetadataAPI.API;
using MetadataAPI.Domain.Entities;
using MetadataAPI.Infrastructure.Persistent;
using Microsoft.Extensions.DependencyInjection;

namespace MetadataAPI.Tests.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private int _recordCount = 3;

        public DatabaseFixture()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            Setup();
        }

        private void Setup()
        {
            SeedDatabase().Wait();
        }

        public async Task CleanupAsync()
        {

            await RemoveLastRecordsFromDatabase(_recordCount);
        }

        public void Dispose()
        {
            CleanupAsync().Wait();
        }

        private async Task SeedDatabase()
        {
            using var scope = new CustomWebApplicationFactory<Program>().Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is clean before seeding
            db.ClinicalTrialMetadata.RemoveRange(db.ClinicalTrialMetadata);
            await db.SaveChangesAsync();

            var testData = new List<ClinicalTrialMetadata>
            {
                new() { TrialId = "T1", Title = "Trial 1", Status = "Ongoing", StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)) },
                new() { TrialId = "T2", Title = "Trial 2", Status = "Completed", StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)) },
                new() { TrialId = "T3", Title = "Trial 3", Status = "Not Started", StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), EndDate = null }
            };

            await db.ClinicalTrialMetadata.AddRangeAsync(testData);
            await db.SaveChangesAsync();
        }
        private async Task RemoveLastRecordsFromDatabase(int recordCount)
        {
            using var scope = new CustomWebApplicationFactory<Program>().Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var records = db.ClinicalTrialMetadata
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(recordCount);

                db.ClinicalTrialMetadata.RemoveRange(records);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing seeded data: {ex.Message}");
            }
        }

        public CustomWebApplicationFactory<Program> Factory => _factory;
    }

}
