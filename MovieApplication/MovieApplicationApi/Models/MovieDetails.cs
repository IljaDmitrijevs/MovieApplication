namespace MovieApplicationApi.Models
{
    /// <summary>
    /// Movie details model for displaying detailed information for specific movie.
    /// </summary>
    public class MovieDetails
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
        /// Movie genre.
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Movie director.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Movie writer.
        /// </summary>
        public string Writer { get; set; } = string.Empty;

        /// <summary>
        /// Movie actors.
        /// </summary>
        public string Actors { get; set; } = string.Empty;

        /// <summary>
        /// Movie plot.
        /// </summary>
        public string Plot { get; set; } = string.Empty;

        /// <summary>
        /// Movie country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Movie poster.
        /// </summary>
        public string Poster { get; set; } = string.Empty;

        /// <summary>
        /// Movie idmg rating.
        /// </summary>
        public string imdbRating { get; set; } = string.Empty;
    }
}
