using FluentAssertions;
using MetadataAPI.API;
using MetadataAPI.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace MetadataAPI.Tests.IntegrationTests
{
    [Collection("Sequential Integration Tests")]
    public class UploadJsonMetadataTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _db;

        public UploadJsonMetadataTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _db = factory.Db;
        }

        private MultipartFormDataContent CreateFileWithValidJsonContent(
            string json = "{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")
        {
            var fileContent = new StringContent(json, Encoding.UTF8, "application/json");

            var formData = new MultipartFormDataContent
            {
                { fileContent, "file", "valid_metadata.json" }
            };

            return formData;
        }

        [Fact]
        public async Task UploadJsonMetadata_ShouldReturnSuccess_WhenValidJson()
        {
            var content = CreateFileWithValidJsonContent();
            var response = await _client.PostAsync("/api/ClinicalTrial/upload", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await RemoveLastRecord();
        }

        [Theory]
        //Valid JSON with all fields
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\",\r\n  \"participants\": 1\r\n}")]
        //Valid JSON with end date not provided and status Ongoing
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"status\": \"Ongoing\",\r\n  \"participants\": 1\r\n}")]
        public async Task UploadJsonMetadata_ShouldReturnSuccess_WhenValidJson_BusinessRulesChecks(string json)
        {
            var content = CreateFileWithValidJsonContent(json);
            var response = await _client.PostAsync("/api/ClinicalTrial/upload", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var record = await _db.ClinicalTrialMetadata
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
            record?.EndDate.Should().NotBeNull();
            record?.DurationInDays.Should().BeGreaterThanOrEqualTo(30);

            await RemoveLastRecord();
        }

        [Fact]
        public async Task UploadJsonMetadata_ShouldReturnBadRequest_WhenInvalidJson()
        {
            var invalidJson = "{ \"trialId\": 123 }"; // Wrong type
            var content = new MultipartFormDataContent
        {
            { new StringContent(invalidJson, Encoding.UTF8, "application/json"), "file" }
        };

            var response = await _client.PostAsync("/api/ClinicalTrial/upload", content);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UploadJsonMetadata_ShouldReturnBadRequest_WhenDuplicateTrialId()
        {
            await _client.PostAsync("/api/ClinicalTrial/upload", CreateFileWithValidJsonContent()); // First upload
            var response = await _client.PostAsync("/api/ClinicalTrial/upload", CreateFileWithValidJsonContent()); // Second upload (duplicate)
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await RemoveLastRecord();
        }

        private async Task RemoveLastRecord()
        {
            try
            {
                var record = await _db.ClinicalTrialMetadata
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefaultAsync();

                if (record != null)
                {
                    _db.ClinicalTrialMetadata.Remove(record);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing last record: {ex.Message}");
            }
        }
    }

}
