using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MovieApplicationApi.Models;

namespace Client.Components.Pages.MovieSearch
{
    /// <summary>
    /// Movie search base logic class for razor page.
    /// </summary>
    public partial class MovieSearchBase : ComponentBase
    {
        [Inject] public IHttpClientFactory ClientFactory { get; set; } = default!;

        protected bool isSearching = false;
        protected bool isDarkMode = false;
        protected string ThemeClass => isDarkMode ? "dark" : "light";

        protected string? searchTitle;
        protected List<Movie>? searchResults;
        protected MovieDetails? selectedMovie;
        protected List<string>? recentSearches;

        protected HttpClient Http => ClientFactory.CreateClient("MovieApplicationApi");

        /// <summary>
        /// On page loading initialize recent searches titles.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                recentSearches = await Http.GetFromJsonAsync<List<string>>("api/movie/recent");
            }
            catch
            {
                recentSearches = new List<string>();
            }
        }

        /// <summary>
        /// Toggle dark/light mode for page.
        /// </summary>
        protected void ToggleDarkMode() => isDarkMode = !isDarkMode;

        /// <summary>
        /// Handles search by pressing "Enter" on keyboard.
        /// </summary>
        /// <param name="e">Keyboard event.</param>
        protected async Task HandleSearchEnter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Search();
            }
        }

        /// <summary>
        /// Executes search by movie title.
        /// </summary>
        /// <param name="title">Movie title.</param>
        protected async Task Search(string? title = null)
        {
            isSearching = true;

            try
            {
                if (!string.IsNullOrEmpty(title))
                    searchTitle = title;

                if (string.IsNullOrWhiteSpace(searchTitle))
                    return;

                var response = await Http.GetFromJsonAsync<MovieSearchResponse>($"api/movie/search?title={searchTitle}");
                searchResults = response?.Search ?? new List<Movie>();

                selectedMovie = null;
                recentSearches = await Http.GetFromJsonAsync<List<string>>("api/movie/recent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching movies: {ex.Message}");
                throw;
            }
            finally
            {
                isSearching = false;
            }
        }

        /// <summary>
        /// Loads details for selected movie by imdb id.
        /// </summary>
        /// <param name="imdbId">Movie imdb id.</param>
        protected async Task LoadDetails(string imdbId)
        {
            var response = await Http.GetFromJsonAsync<MovieDetails>($"api/movie/details?imdbId={imdbId}");
            if (response != null)
                selectedMovie = response;
        }
    }
}
