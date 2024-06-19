using OKEInterviewApp.Models;

namespace OKEInterviewApp.Services;

public interface IMovieService
{
    Task<IEnumerable<Movie>> GetMoviesAsync(string source);
}
