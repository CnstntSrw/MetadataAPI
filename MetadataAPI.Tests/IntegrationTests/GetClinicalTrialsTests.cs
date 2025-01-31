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
    public class GetClinicalTrialsTests : IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;

        public GetClinicalTrialsTests()
        {
            _client = new DatabaseFixture().Factory.CreateClient();
        }

        [Fact]
        public async Task GetTrials_ShouldReturnAllTrials_WhenNoFiltersApplied()
        {
            var response = await _client.GetAsync("/api/ClinicalTrial");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trials = await response.Content.ReadFromJsonAsync<List<ClinicalTrialMetadata>>();
            trials.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetTrials_ShouldFilterByStatus()
        {
            var response = await _client.GetAsync("/api/ClinicalTrial?status=Ongoing");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trials = await response.Content.ReadFromJsonAsync<List<ClinicalTrialMetadata>>();
            trials.Should().HaveCount(1);
            trials[0].TrialId.Should().Be("T1");
        }

        [Fact]
        public async Task GetTrials_ShouldFilterByStartDate()
        {
            var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)).ToString("yyyy-MM-dd");
            var response = await _client.GetAsync($"/api/ClinicalTrial?startDate={startDate}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trials = await response.Content.ReadFromJsonAsync<List<ClinicalTrialMetadata>>();
            trials.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetTrials_ShouldFilterByEndDate()
        {
            var endDate = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
            var response = await _client.GetAsync($"/api/ClinicalTrial?endDate={endDate}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trials = await response.Content.ReadFromJsonAsync<List<ClinicalTrialMetadata>>();
            trials.Should().HaveCount(1);
            trials[0].TrialId.Should().Be("T2");
        }

        [Fact]
        public async Task GetTrials_ShouldFilterByMultipleCriteria()
        {
            var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)).ToString("yyyy-MM-dd");
            var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)).ToString("yyyy-MM-dd");

            var response = await _client.GetAsync($"/api/ClinicalTrial?status=Ongoing&startDate={startDate}&endDate={endDate}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var trials = await response.Content.ReadFromJsonAsync<List<ClinicalTrialMetadata>>();
            trials.Should().HaveCount(1);
            trials[0].TrialId.Should().Be("T1");
        }
    }

}
