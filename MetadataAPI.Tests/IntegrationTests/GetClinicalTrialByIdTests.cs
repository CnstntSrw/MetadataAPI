using MetadataAPI.Tests.IntegrationTests.Fixtures;

namespace MetadataAPI.Tests.IntegrationTests
{
    using FluentAssertions;
    using MetadataAPI.Domain.Entities;
    using System.Net;
    using System.Net.Http.Json;
    using Xunit;

    [Collection("Sequential Integration Tests")]
    [assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
    public class GetClinicalTrialByIdTests : DatabaseFixture
    {
        private readonly HttpClient _client;

        public GetClinicalTrialByIdTests()
        {
            _client = new DatabaseFixture().Factory.CreateClient();
        }

        [Fact]
        public async Task GetTrialById_ShouldReturnTrial_WhenExists()
        {
            var response = await _client.GetAsync("/api/ClinicalTrial/T1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trial = await response.Content.ReadFromJsonAsync<ClinicalTrialMetadata>();
            trial.Should().NotBeNull();
            trial.TrialId.Should().Be("T1");
        }

        [Fact]
        public async Task GetTrialById_ShouldReturnNotFound_WhenTrialDoesNotExist()
        {
            var response = await _client.GetAsync("/api/ClinicalTrial/INVALID_ID");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

}
