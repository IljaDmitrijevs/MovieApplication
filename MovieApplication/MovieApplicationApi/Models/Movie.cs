namespace MovieApplicationApi.Models
{
    /// <summary>
    /// Movie short model for search.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Movie title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Movie year.
        /// </summary>
        public string Year { get; set; } = string.Empty;

        /// <summary>
        /// Movie imdb id.
        /// </summary>
        public string imdbID { get; set; } = string.Empty;
    }
}
    