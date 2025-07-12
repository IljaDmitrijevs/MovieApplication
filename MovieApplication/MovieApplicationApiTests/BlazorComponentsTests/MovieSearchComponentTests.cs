using Bunit;
using Client.Components.Pages.MovieSearch;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using MovieApplicationApi.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace MovieApplicationTests.BlazorComponentsTests
{
    public class MovieSearchComponentTests : TestContext
    {
        private readonly MockHttpMessageHandler _mockHttp;

        private readonly string BaseUri = "https://localhost:7136/";

        public MovieSearchComponentTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            _mockHttp.ResetBackendDefinitions();

            var client = _mockHttp.ToHttpClient();
            client.BaseAddress = new Uri(BaseUri);

            Services.RemoveAll<HttpClient>();
            Services.RemoveAll<IHttpClientFactory>();

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient("MovieApplicationApi"))
                                 .Returns(client);

            Services.AddSingleton(httpClientFactoryMock.Object);

            _mockHttp.When("/api/movie/recent")
                     .Respond("application/json", JsonSerializer.Serialize(new List<string> { "Matrix", "Batman" }));

            _mockHttp.When("/api/movie/search*")
                     .Respond("application/json", JsonSerializer.Serialize(
                         new MovieSearchResponse
                         {
                             Search = new List<Movie>
                             {
                                new Movie { Title = "Matrix", Year = "1999", imdbID = "tt0133093" }
                             }
                         }));

            _mockHttp.When("/api/movie/details")
                     .Respond("application/json", JsonSerializer.Serialize(
                         new MovieDetails
                         {
                             Title = "Matrix",
                             Year = "1999",
                             Genre = "Sci-Fi",
                             Director = "Wachowski",
                             Writer = "Wachowski",
                             Actors = "Keanu Reeves",
                             Country = "USA",
                             Plot = "Neo learns the truth",
                             imdbRating = "8.7",
                             Poster = "https://example.com/matrix.jpg"
                         }));

            // Catch-all for debugging
            _mockHttp.Fallback.Respond(req =>
            {
                Console.WriteLine($"❌ UNMATCHED REQUEST: {req.RequestUri}");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        }


        [Fact]
        public void PageLoaded_RendersComponent_ShowsRecentSearches()
        {
            // Arrange
            var cut = RenderComponent<MoviesSearch>();

            // Act
            cut.WaitForState(() => cut.Markup.Contains("Recent Searches"), timeout: TimeSpan.FromSeconds(5));

            // Assert
            cut.Markup.Should().Contain("Recent Searches");
            cut.Markup.Should().Contain("Matrix");
            cut.Markup.Should().Contain("Batman");
        }

        [Fact]
        public async Task SearchButton_SearchIsDone_FetchesAndDisplaysResults()
        {
            // Arrange
            var cut = RenderComponent<MoviesSearch>();

            // Act
            await cut.InvokeAsync(() =>
            {
                var input = cut.Find("input");
                input.Input("Matrix");
                input.KeyDown(new KeyboardEventArgs { Key = "Enter" });
            });
            cut.WaitForState(() => cut.Markup.Contains("Results:"), timeout: TimeSpan.FromSeconds(5));

            // Assert
            cut.Markup.Should().Contain("Results:");
            cut.Markup.Should().Contain("Matrix (1999)");
        }

        [Fact]
        public async Task MovieLoadsDetails_ClickingOnMovie_ReturnsMovieDetails()
        {
            // Arrange
            var cut = RenderComponent<MoviesSearch>();

            // Act
            await cut.InvokeAsync(() =>
            {
                var input = cut.Find("input");
                input.Input("Matrix");
                input.KeyDown(new KeyboardEventArgs { Key = "Enter" });
            });

            // Wait for search results to appear
            cut.WaitForState(() => cut.FindAll("li").Any(li => li.TextContent.Contains("Matrix (1999)")), timeout: TimeSpan.FromSeconds(5));

            await cut.InvokeAsync(() =>
            {
                var allListItems = cut.FindAll("li");
                var movieItem = allListItems.FirstOrDefault(li => li.TextContent.Contains("Matrix (1999)"));
                movieItem.Click();
            });


            // Wait for details to load
            var details = cut.WaitForElement(".movie-details", timeout: TimeSpan.FromSeconds(5));

            // Assert
            cut.WaitForAssertion(() =>
            {
                var details = cut.Find(".movie-details");
                details.Should().NotBeNull();
                details.TextContent.Should().Contain("Neo learns the truth");
                details.TextContent.Should().Contain("Sci-Fi");
                details.TextContent.Should().Contain("Wachowski");
                details.TextContent.Should().Contain("Keanu Reeves");
                details.TextContent.Should().Contain("USA");
                details.TextContent.Should().Contain("⭐ IMDB: 8.7");
            }, timeout: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void ToggleDarkMode_ChangesTheme()
        {
            // Arrange
            var cut = RenderComponent<MoviesSearch>();

            // Act
            var toggleButton = cut.Find("button.icon-btn");
            toggleButton.Click();

            cut.Markup.Should().Contain("theme-container dark");

            toggleButton.Click();

            // Assert
            cut.Markup.Should().Contain("theme-container light");
        }
    }
}
