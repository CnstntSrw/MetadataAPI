using FluentAssertions;
using MetadataAPI.Application.Commands.UploadJSONMetadata;
using MetadataAPI.Common;
using MetadataAPI.Infrastructure.Interfaces;
using MetadataAPI.Infrastructure.Persistent;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace MetadataAPI.Tests.UnitTests.Application.Handlers
{
    public class UploadJsonMetadataCommandHandlerTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<IJsonSchemaValidator> _jsonValidatorMock;
        private readonly Mock<ILogger<UploadJsonMetadataCommandHandler>> _loggerMock;
        private readonly UploadJsonMetadataCommandHandler _handler;

        public UploadJsonMetadataCommandHandlerTests()
        {
            _jsonValidatorMock = new Mock<IJsonSchemaValidator>();
            _loggerMock = new Mock<ILogger<UploadJsonMetadataCommandHandler>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _handler = new UploadJsonMetadataCommandHandler(_dbContext, _jsonValidatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenFileIsEmpty()
        {
            // Arrange
            var command = new UploadJsonMetadataCommand(null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("No file uploaded.");
        }

        [Theory]
        //trialId type mismatch
        [InlineData("{\r\n  \"trialId\": 123,\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")]
        //No trialId
        [InlineData("{\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")]
        //No title
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")]
        //No startDate
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")]
        //No status
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\"}")]
        //Wrong status
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"asd\"\r\n}")]
        //Participants = 0
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\",\r\n  \"participants\": 0\r\n}")]
        public async Task Handle_ShouldFail_WhenJsonIsInvalid(string jsonData)
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(jsonData.Length);
            fileMock.Setup(f => f.FileName).Returns("invalid.json");

            _jsonValidatorMock.Setup(v => v.ValidateJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(new JsonValidationResult(false, new List<string> { "Invalid type" }));

            var command = new UploadJsonMetadataCommand(fileMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Invalid JSON format");
        }

        [Theory]
        [InlineData("{\r\n  \"trialId\": \"adipisicing nisi Lorem\",\r\n  \"title\": \"minim Duis\",\r\n  \"startDate\": \"1893-07-05\",\r\n  \"endDate\": \"1906-02-27\",\r\n  \"status\": \"Ongoing\"\r\n}")]
        public async Task Handle_ShouldPass_WhenJsonIsValid(string jsonData)
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(jsonData.Length);
            fileMock.Setup(f => f.FileName).Returns("valid.json");

            _jsonValidatorMock.Setup(v => v.ValidateJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(new JsonValidationResult(true, new List<string>()));

            var command = new UploadJsonMetadataCommand(fileMock.Object);


            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
