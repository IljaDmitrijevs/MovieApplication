using Microsoft.AspNetCore.Mvc;
using MovieApplicationApi.Exceptions;
using MovieApplicationApi.Interfaces;
using MovieApplicationApi.Models;

namespace MovieApplicationApi.Controllers
{
    /// <summary>
    /// Movie controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        /// <summary>
        /// Movie logic instance.
        /// </summary>
        private readonly IMovieLogic _movieLogic;

        /// <summary>
        /// Movie controller.
        /// </summary>
        /// <param name="movieLogic"></param>
        public MovieController(IMovieLogic movieLogic)
        {
            _movieLogic = movieLogic;
        }

        /// <summary>
        /// Initates movie search by provided title.
        /// </summary>
        /// <param name="title">Movei title.</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search(string title)
        {
            try
            {
                var result = await _movieLogic.SearchMovies(title);
                await _movieLogic.SaveSearchesToDb(title);

                return Ok(result);
            }
            catch (MovieNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
        }
        
        /// <summary>
        /// Gets recent searches.
        /// </summary>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentSearches()
        {
            var result = await _movieLogic.GetDistinctTitles();

            return Ok(result);
        }

        /// <summary>
        /// Gets specific movie details by provided imdb id.
        /// </summary>
        /// <param name="imdbId">Movie imdb id.</param>
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails(string imdbId)
        {
            try
            {
                var result = await _movieLogic.GetMovieDetails(imdbId);

                return Ok(result);
            }
            catch (MovieNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
        }
    }
}
