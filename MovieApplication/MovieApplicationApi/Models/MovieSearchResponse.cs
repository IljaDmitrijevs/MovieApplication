namespace MovieApplicationApi.Models
{
    /// <summary>
    /// Movies search result model for displaying list of movies found.
    /// </summary>
    public class MovieSearchResponse
    {
        /// <summary>
        /// Movies list search result.
        /// </summary>
        public required List<Movie> Search { get; set; }

        /// <summary>
        /// Movies total search result.
        /// </summary>
        public string TotalResults { get; set; } = string.Empty;
    }
}
