namespace MovieApplicationApi.Models
{
    /// <summary>
    /// Omdb settings model.
    /// </summary>
    public class OmdbSettings
    {
        /// <summary>
        /// Omdb api key.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Omdb base url.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
    }
}
