namespace MovieApplicationApi.Entities
{
    /// <summary>
    /// Movie search db model.
    /// </summary>
    public class MovieSearch
    {
        /// <summary>
        /// Movie search record id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Movie title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Datetime when movie search was executed.
        /// </summary>
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    }
}
