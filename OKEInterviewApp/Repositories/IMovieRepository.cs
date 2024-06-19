using OKEInterviewApp.Models;

namespace OKEInterviewApp.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMoviesAsync();
}
