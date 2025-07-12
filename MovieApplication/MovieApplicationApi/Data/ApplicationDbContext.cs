using Microsoft.EntityFrameworkCore;
using MovieApplicationApi.Entities;

namespace MovieApplicationApi.Data
{
    /// <summary>
    /// Movie application database context.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Movie searches.
        /// </summary>
        public DbSet<MovieSearch> MovieSearches { get; set; }

        /// <summary>
        /// Movie application database context.
        /// </summary>
        /// <param name="options">Options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }
}
