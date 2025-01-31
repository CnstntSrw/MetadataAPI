using FluentAssertions;
using MetadataAPI.Application.Interfaces;
using MetadataAPI.Application.Queries.GetClinicalTrials;
using MetadataAPI.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace MetadataAPI.Tests.UnitTests.Application.Handlers
{
    public class GetClinicalTrialsQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly GetClinicalTrialsQueryHandler _handler;
        private readonly List<ClinicalTrialMetadata> _mockData;

        public GetClinicalTrialsQueryHandlerTests()
        {
            _dbContextMock = new Mock<IApplicationDbContext>();

            _mockData = new List<ClinicalTrialMetadata>
        {
            new ClinicalTrialMetadata { TrialId = "T1", Status = "Ongoing", StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(20)) },
            new ClinicalTrialMetadata { TrialId = "T2", Status = "Completed", StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)) },
            new ClinicalTrialMetadata { TrialId = "T3", Status = "Not Started", StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), EndDate = null }
        };

            var mockSet = _mockData.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(db => db.ClinicalTrialMetadata).Returns(mockSet.Object);

            _handler = new GetClinicalTrialsQueryHandler(_dbContextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllTrials_WhenNoFiltersAreApplied()
        {
            // Arrange
            var query = new GetClinicalTrialsQuery(null, null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_ShouldFilterByStatus()
        {
            // Arrange
            var query = new GetClinicalTrialsQuery("Ongoing", null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().TrialId.Should().Be("T1");
        }

        [Fact]
        public async Task Handle_ShouldFilterByStartDate()
        {
            // Arrange
            var query = new GetClinicalTrialsQuery(null, DateOnly.FromDateTime(DateTime.Today.AddDays(-20)), null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2); // T1 and T2
        }

        [Fact]
        public async Task Handle_ShouldFilterByEndDate()
        {
            // Arrange
            var query = new GetClinicalTrialsQuery(null, null, DateOnly.FromDateTime(DateTime.Today));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1); // Only T2
        }

        [Fact]
        public async Task Handle_ShouldFilterByStatusAndDate()
        {
            // Arrange
            var query = new GetClinicalTrialsQuery("Ongoing", DateOnly.FromDateTime(DateTime.Today.AddDays(-20)), DateOnly.FromDateTime(DateTime.Today.AddDays(20)));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().TrialId.Should().Be("T1");
        }
    }

}
