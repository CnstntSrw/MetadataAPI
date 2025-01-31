using FluentAssertions;
using MetadataAPI.Infrastructure.Services;
using Xunit;

namespace MetadataAPI.Tests.UnitTests.Infrastructure.Services
{
    public class JsonSchemaValidatorTests
    {
        private readonly JsonSchemaValidator _validator;

        public JsonSchemaValidatorTests()
        {
            _validator = new JsonSchemaValidator(); // Ensure it loads the correct schema
        }

        [Fact]
        public async Task ValidateJsonAsync_ShouldReturnSuccess_WhenJsonIsValid()
        {
            // Arrange
            var validJson = "{ \"trialId\": \"T123\", \"title\": \"Test\", \"startDate\": \"2024-01-01\", \"status\": \"Ongoing\" }";

            // Act
            var result = await _validator.ValidateJsonAsync(validJson);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateJsonAsync_ShouldReturnFailure_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "{ \"trialId\": 123, \"title\": 456 }"; // Wrong types

            // Act
            var result = await _validator.ValidateJsonAsync(invalidJson);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }

}
