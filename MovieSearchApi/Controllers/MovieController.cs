using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MovieSearchApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieController :ControllerBase
    {
        private readonly OmdbService _omdbService;

        public MovieController(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<Movie>> SearchMovieByTitle(string title)
        {
            // Implement logic to search for a movie by title using the OmdbService
        }

        [HttpGet("latest-searches")]
        public ActionResult<IEnumerable<SearchQuery>> GetLatestSearchQueries()
        {
            // Implement logic to retrieve and return the latest search queries
        }

    }
}
