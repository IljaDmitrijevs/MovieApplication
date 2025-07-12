namespace MovieApplicationApi.Exceptions
{
    public class MovieNotFoundException : Exception
    {
        public MovieNotFoundException(string title)
            : base($"No movies found for title: '{title}'.") { }

        public MovieNotFoundException()
            : base("Movie not found.") { }
    }
}
