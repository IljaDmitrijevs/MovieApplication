namespace MovieApplicationApi.Models
{
    /// <summary>
    /// Error response for HHTP request.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }
}
