using MovieApplicationApi.Models;

namespace MovieApplicationApi.Interfaces
{
    /// <summary>
    /// Movie logic interface.
    /// </summary>
    public interface IMovieLogic
    {
        /// <summary>
        /// Search movies by provided title.
        /// </summary>
        /// <param name="title">Movie title.</param>
        Task<MovieSearchResponse> SearchMovies(string title);

        /// <summary>
        /// Gets specific movie details by provided imdb id.
        /// </summary>
        /// <param name="imdbId">imdb id.</param>
        Task<MovieDetails> GetMovieDetails(string imdbId);

        /// <summary>
        /// Gets distinct titles for recent search result.
        /// </summary>
        Task<List<string>> GetDistinctTitles();
        
        /// <summary>
        /// Saves searches to database.
        /// </summary>
        /// <param name="title">Movie title.</param>
        Task SaveSearchesToDb(string title);
    }
}