using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using MovieApplicationApi.Data;
using MovieApplicationApi.Entities;
using MovieApplicationApi.Exceptions;
using MovieApplicationApi.Models;
using System.Net;
using System.Text.Json;

namespace MovieApplicationTests.ApiTests.LogicTests
{
    public class MovieLogicTests
    {
        private readonly MovieLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly OmdbSettings _settings = new() { ApiKey = "test", BaseUrl = "http://test.com" };

        public MovieLogicTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            var client = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri(_settings.BaseUrl)
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieDbTest")
                .Options;

            _context = new ApplicationDbContext(options);

            _httpClient = client;

            var optionsMock = Options.Create(_settings);
            _logic = new MovieLogic(_httpClient, optionsMock, _context);
        }


        [Fact]
        public async Task SearchMovies_ValidTitle_CallsHttpClientAndReturnsResult()
        {
            // Arrange
            var title = "Inception";
            var expected = new MovieSearchResponse { Search = new List<Movie> { new() { Title = "Inception" } } };
            var json = JsonSerializer.Serialize(expected);

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var result = await _logic.SearchMovies(title);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task SearchMovies_ResponseJsonIsNull_ThrowsMovieNotFoundException()
        {
            // Arrange
            var title = "Inception";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("null")
                });

            // Act & Assert
            await Assert.ThrowsAsync<MovieNotFoundException>(() => _logic.SearchMovies(title));
        }


        [Fact]
        public async Task GetMovieDetails_ValidId_CallsHttpClientAndReturnsResult()
        {
            // Arrange
            var imdbId = "tt123";
            var expected = new MovieDetails
            {
                Title = "Inception",
                Year = "2010",
                Genre = "Action",
                Director = "Some director",
                Writer = "Some writer",
                Actors = "Leonardo DiCaprio",
                Plot = "Some plot",
                Country = "Latvia",
                Poster = "poster",
                imdbRating = "100",
            };
            var json = JsonSerializer.Serialize(expected);

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var result = await _logic.GetMovieDetails(imdbId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetMovieDetails_ResponseJsonIsNull_ThrowsMovieNotFoundException()
        {
            // Arrange
            var imdbId = "tt123456";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("null")
                });

            // Act & Assert
            await Assert.ThrowsAsync<MovieNotFoundException>(() => _logic.GetMovieDetails(imdbId));
        }


        [Fact]
        public async Task SaveSearchesToDb_AddsEntry_EnrtyHasCorrectTitle()
        {
            // Arrange
            var context = GetDbContext();
            var logic = GetLogic(context);
            var title = "The Matrix";

            // Act
            await logic.SaveSearchesToDb(title);

            // Assert
            var result = context.MovieSearches.FirstOrDefault(x => x.Title == title);
            result.Should().NotBeNull();
            result.Title.Should().Be(title);
        }

        [Fact]
        public async Task GetDistinctTitles_MethodIsCalled_ReturnsFivetRecentDistinctTitles()
        {
            // Arrange
            var context = GetDbContext();
            var logic = GetLogic(context);

            context.MovieSearches.AddRange(new[]
            {
            new MovieSearch { Title = "Prison", SearchedAt = DateTime.UtcNow.AddMinutes(-1) },
            new MovieSearch { Title = "Matrix", SearchedAt = DateTime.UtcNow.AddMinutes(-2) },
            new MovieSearch { Title = "Batman", SearchedAt = DateTime.UtcNow.AddMinutes(-3) },
            new MovieSearch { Title = "Inception", SearchedAt = DateTime.UtcNow.AddMinutes(-4) },
            new MovieSearch { Title = "Spider-man", SearchedAt = DateTime.UtcNow.AddMinutes(-5) },
            new MovieSearch { Title = "Walking dead", SearchedAt = DateTime.UtcNow.AddMinutes(-6) },
        });
            await context.SaveChangesAsync();

            // Act
            var result = await logic.GetDistinctTitles();

            // Assert
            result.Count.Should().Be(5);
            result[0].Should().Be("Prison");
            result[1].Should().Be("Matrix");
            result[2].Should().Be("Batman");
            result[3].Should().Be("Inception");
            result[4].Should().Be("Spider-man");
        }

        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private MovieLogic GetLogic(ApplicationDbContext context)
        {
            var httpClient = new HttpClient();
            var omdbOptions = Options.Create(new OmdbSettings
            {
                ApiKey = _settings.ApiKey,
                BaseUrl = _settings.BaseUrl,
            });

            return new MovieLogic(httpClient, omdbOptions, context);
        }
    }
}