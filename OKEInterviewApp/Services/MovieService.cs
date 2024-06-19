using Microsoft.Extensions.Caching.Memory;
using OKEInterviewApp.Models;
using OKEInterviewApp.Repositories;
using Serilog;

namespace OKEInterviewApp.Services;

public class MovieService : IMovieService
{
    private readonly IEnumerable<IMovieRepository> _repositories;
    private readonly IMemoryCache _cache;

    public MovieService(IEnumerable<IMovieRepository> repositories, IMemoryCache cache)
    {
        _repositories = repositories;
        _cache = cache;
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync(string source)
    {
        if (_cache.TryGetValue(source, out IEnumerable<Movie> movies))
        {
            return movies;
        }

        var repository = _repositories.FirstOrDefault(repo => repo.GetType().Name.Contains(source, StringComparison.OrdinalIgnoreCase));

        if (repository == null)
        {
            Log.Warning($"Repository for source '{source}' not found.");
            return Enumerable.Empty<Movie>();
        }

        movies = await repository.GetMoviesAsync();
        _cache.Set(source, movies, TimeSpan.FromMinutes(30));

        return movies;
    }
}
