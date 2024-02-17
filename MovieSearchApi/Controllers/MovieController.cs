using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services;

namespace MovieSearchApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieController : ControllerBase
    {
        private readonly OmdbService _omdbService;
        private readonly IMemoryCache _cache;

        public MovieController(OmdbService omdbService, IMemoryCache cache)
        {
            _omdbService = omdbService;
            _cache = cache;
        }

        [HttpGet("search")]
        public async Task<ActionResult<Movie>> SearchMovie(string id, string title, string type, string year)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return Ok(await _omdbService.SearchMovieByIdAsync(id));
                }

                if (!string.IsNullOrEmpty(title))
                {
                    Movie movie = await _omdbService.SearchMovieByTitleAsync(title, type, year);
                    SaveSearchQuery(title);

                    return Ok(movie);
                }

                return BadRequest("Please provide valid parameters (ID or Title).");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private void SaveSearchQuery(string title)
        {
            var searchQuery = new SearchQuery { Title = title, SearchTime = DateTime.Now };
            var latestSearchQueries = _cache.Get<List<SearchQuery>>("LatestSearchQueries") ?? new List<SearchQuery>();
            latestSearchQueries.Add(searchQuery);
            _cache.Set("LatestSearchQueries", latestSearchQueries, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }
    }
}
