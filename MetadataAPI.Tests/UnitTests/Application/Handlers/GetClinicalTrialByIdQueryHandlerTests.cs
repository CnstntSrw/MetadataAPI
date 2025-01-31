using FluentAssertions;
using MetadataAPI.Application.Interfaces;
using MetadataAPI.Application.Queries.GetClinicalTrialById;
using MetadataAPI.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace MetadataAPI.Tests.UnitTests.Application.Handlers
{
    public class GetClinicalTrialByIdQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly GetClinicalTrialByIdQueryHandler _handler;

        public GetClinicalTrialByIdQueryHandlerTests()
        {
            _dbContextMock = new Mock<IApplicationDbContext>();
            _handler = new GetClinicalTrialByIdQueryHandler(_dbContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrial_WhenFound()
        {
            // Arrange
            var trial = new ClinicalTrialMetadata { TrialId = "T123", Title = "Test Trial" };
            var mockSet = new List<ClinicalTrialMetadata> { trial }
                .AsQueryable()
                .BuildMockDbSet();

            _dbContextMock.Setup(db => db.ClinicalTrialMetadata).Returns(mockSet.Object);

            var query = new GetClinicalTrialByIdQuery("T123");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TrialId.Should().Be("T123");
        }


        [Fact]
        public async Task Handle_ShouldReturnNull_WhenTrialNotFound()
        {
            // Arrange
            var mockSet = new List<ClinicalTrialMetadata>().AsQueryable()
                .BuildMockDbSet();

            _dbContextMock.Setup(db => db.ClinicalTrialMetadata).Returns(mockSet.Object);

            var query = new GetClinicalTrialByIdQuery("T123");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }

}
