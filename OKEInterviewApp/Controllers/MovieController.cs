using Microsoft.AspNetCore.Mvc;
using OKEInterviewApp.Models;
using OKEInterviewApp.Services;

namespace OKEInterviewApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Retrieves a list of movies based on the specified source.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// GET /api/movies?source=staticfile
    /// </remarks>
    /// <param name="source">The source from which movies should be retrieved. Options: StaticFile or IMDbOTApi</param>
    /// <returns>A list of movies from the specified source.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesAsync([FromQuery] string source)
    {
        var movies = await _movieService.GetMoviesAsync(source);

        return Ok(movies);
    }
}
