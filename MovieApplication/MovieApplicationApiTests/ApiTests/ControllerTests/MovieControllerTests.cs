using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApplicationApi.Controllers;
using MovieApplicationApi.Exceptions;
using MovieApplicationApi.Interfaces;
using MovieApplicationApi.Models;

namespace MovieApplicationTests.ApiTests.ControllerTests
{
    public class MovieControllerTests
    {
        private readonly Mock<IMovieLogic> _mockMovieLogic;
        private readonly MovieController _controller;

        public MovieControllerTests()
        {
            _mockMovieLogic = new Mock<IMovieLogic>();
            _controller = new MovieController(_mockMovieLogic.Object);
        }

        [Fact]
        public async Task Search_ValidTitle_ReturnsOkWithResults_AndSavesSearch()
        {
            // Arrange
            var title = "Inception";
            var expected = new MovieSearchResponse
            {
                Search = new List<Movie>
            {
                new Movie { Title = "Inception", Year = "2010", imdbID = "tt1375666" }
            }
            };

            _mockMovieLogic.Setup(m => m.SearchMovies(title)).ReturnsAsync(expected);
            _mockMovieLogic.Setup(m => m.SaveSearchesToDb(title)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Search(title);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expected);

            _mockMovieLogic.Verify(m => m.SaveSearchesToDb(title), Times.Once);
        }

        [Fact]
        public async Task GetDetails_ValidId_ReturnsMovieDetails()
        {
            // Arrange
            var imdbId = "tt1375666";
            var expected = new MovieDetails
            {
                Title = "Inception",
                imdbRating = "8.8",
                Year = "2010",
                Genre = "Action",
                Actors = "Leonardo DiCaprio"
            };

            _mockMovieLogic.Setup(m => m.GetMovieDetails(imdbId)).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetDetails(imdbId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetDetails_MovieNotFound_ReturnsNotFound()
        {
            // Arrange
            var imdbId = "tt0000000";
            var errorMessage = "Movie not found.";

            _mockMovieLogic
                .Setup(m => m.GetMovieDetails(imdbId))
                .ThrowsAsync(new MovieNotFoundException());

            // Act
            var result = await _controller.GetDetails(imdbId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            error.Error.Should().Be(errorMessage);
        }

        [Fact]
        public async Task Search_MovieNotFound_ReturnsNotFound()
        {
            // Arrange
            var title = "UnknownMovie";
            var errorMessage = $"No movies found for title: '{title}'.";

            _mockMovieLogic
                .Setup(m => m.SearchMovies(title))
                .ThrowsAsync(new MovieNotFoundException(title));

            // Act
            var result = await _controller.Search(title);

            // Asserts
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            error.Error.Should().Be(errorMessage);

            _mockMovieLogic.Verify(m => m.SaveSearchesToDb(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetRecentSearches_ReturnsDistinctTitles()
        {
            // Arrange
            var expected = new List<string> { "Matrix", "Batman", "Inception" };

            _mockMovieLogic.Setup(m => m.GetDistinctTitles()).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetRecentSearches();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expected);
        }
    }
}