using AutoMapper;
using Bogus;
using MockQueryable.Moq;
using Moq;
using ShowsTracker.Application;
using ShowsTracker.Application.Service.Show;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Enum;
using ShowsTracker.Common.Helpers;
using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Tests.Show
{
    public class ShowServiceTests
    {
        private readonly ShowService _showService;
        private readonly Mock<IRepository<ShowsTracker.Domain.Show>> _showRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IClaimManager> _claimManagerMock;
        private readonly Mock<IRepository<ShowsTracker.Domain.ShowLink>> _showLinkRepositoryMock;
        public ShowServiceTests()
        {
            _showRepositoryMock = new Mock<IRepository<Domain.Show>>();
            _mapperMock = new Mock<IMapper>();
            _claimManagerMock = new Mock<IClaimManager>();
            _showLinkRepositoryMock = new Mock<IRepository<Domain.ShowLink>>();
            _showService = new ShowService(
                _showRepositoryMock.Object,
                _mapperMock.Object,
                _claimManagerMock.Object,
                _showLinkRepositoryMock.Object);
        }

        public static List<Domain.Show> GenerateMockShows(int count)
        {
            var showFaker = new Faker<Domain.Show>()
                .RuleFor(s => s.Id, f => f.Random.Guid())
                .RuleFor(s => s.Name, f => f.Random.Word())
                .RuleFor(s => s.TotalEpisode, f => f.Random.Number(1, 100))
                .RuleFor(s => s.Status, f => f.PickRandom<Status>())
                .RuleFor(s => s.Description, f => f.Lorem.Paragraph())
                .RuleFor(s => s.ApproveStatus, f => f.Random.Bool())
                .RuleFor(s => s.CoverImageUrl, f => f.Internet.Url())
                .RuleFor(s => s.BannerImageUrl, f => f.Internet.Url())
                .RuleFor(s => s.Rating, f => f.Random.Double(1, 10))
                .RuleFor(s => s.Rank, f => f.Random.Number(1, 100))
                .RuleFor(s => s.Popularity, f => f.Random.Number(1, 1000))
                .RuleFor(s => s.StartDate, f => f.Date.Past())
                .RuleFor(s => s.EndDate, f => f.Date.Future())
                .RuleFor(s => s.ReleaseGap, f => f.Random.Number(1, 30))
                .RuleFor(s => s.MyAnimeListId, f => f.Random.Number(1, 10000))
                .RuleFor(s => s.Links, (f, e) => GenerateMockShowLinks(f.Random.Number(1, 5), e.Id))
                .RuleFor(s => s.IsDeleted, f => f.Random.Bool())
                .RuleFor(s => s.InsertedUserId, f => f.Random.Guid())
                .RuleFor(s => s.Users, f => new Faker<ShowUser>().Generate(0))
                .RuleFor(s => s.Genres, f => new Faker<ShowGenre>().Generate(0))
                .RuleFor(s => s.InsertedUserId, f => f.Random.Guid());


            return showFaker.Generate(count);
        }
        private static List<ShowLink> GenerateMockShowLinks(int count, Guid showId)
        {
            var showLinkFaker = new Faker<ShowLink>()
                .RuleFor(sl => sl.Id, f => f.Random.Guid())
                .RuleFor(sl => sl.Link, f => f.Internet.Url())
                .RuleFor(sl => sl.UserId, f => f.Random.Guid())
                .RuleFor(sl => sl.IsDefaultLink, f => f.Random.Bool())
                .RuleFor(sl => sl.Name, f => f.Random.Word())
                .RuleFor(sl => sl.IsDeleted, f => f.Random.Bool())
                .RuleFor(sl => sl.ShowId, f => showId);

            return showLinkFaker.Generate(count);
        }

        [Fact]
        public async Task GetAllShows_ReturnsSuccessResponse()
        {
            // Arrange
            var claimManagerMock = new Mock<IClaimManager>();
            claimManagerMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

            var mockData = GetMockShows().AsQueryable().BuildMock();
            _showRepositoryMock.Setup(x => x.GetAll()).Returns(mockData);

            _showLinkRepositoryMock.Setup(x => x.GetAll()).Returns(mockData.SelectMany(f => f.Links).AsQueryable().BuildMock());

            var request = new GetAllShowsDto(); // Provide a valid request

            // Act
            var result = await _showService.GetAllShows(request);

            // Assert
            Assert.True(result.IsSuccesfull);
            Assert.NotNull(result.Data);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public async Task GetAllShows_ReturnsFailureResponse_WhenRepositoryThrowsException()
        {
            // Arrange
            var claimManagerMock = new Mock<IClaimManager>();
            claimManagerMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

            _showRepositoryMock.Setup(x => x.GetAll()).Throws(new Exception("Simulated repository exception"));

            var request = new GetAllShowsDto(); // Provide a valid request

            // Act
            var result = await _showService.GetAllShows(request);

            // Assert
            Assert.False(result.IsSuccesfull);
            Assert.Null(result.Data);
            Assert.Equal("Simulated repository exception", result.ErrorMessage);
        }

        // Add more tests for different failure scenarios as needed

        // Helper method to generate mock shows
        private List<Domain.Show> GetMockShows()
        {
            return GenerateMockShows(50);
        }
    }
}
