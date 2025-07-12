using Microsoft.AspNetCore.Mvc;
using MovieApplicationApi.Exceptions;
using MovieApplicationApi.Interfaces;
using MovieApplicationApi.Models;

namespace MovieApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieLogic _movieLogic;

        public MovieController(IMovieLogic movieLogic)
        {
            _movieLogic = movieLogic;
        }

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

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentSearches()
        {
            var result = await _movieLogic.GetDistinctTitles();

            return Ok(result);
        }


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
