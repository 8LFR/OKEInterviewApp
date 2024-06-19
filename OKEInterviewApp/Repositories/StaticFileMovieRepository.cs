using OKEInterviewApp.Models;
using Serilog;
using System.Text.Json;

namespace OKEInterviewApp.Repositories;

public class StaticFileMovieRepository : IMovieRepository
{
    private readonly List<Movie> movies;
    private static readonly string filePath = Path.Combine(Environment.CurrentDirectory, "StaticFiles\\MovieList.json");

    public StaticFileMovieRepository()
    {
        try
        {
            movies = GetMoviesFromJsonFile(filePath);
        }
        catch (FileNotFoundException ex)
        {
            Log.Error(ex, "The specified JSON file was not found.");
            movies = new List<Movie>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while reading the JSON file.");
            movies = new List<Movie>();
        }
    }

    public Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        return Task.FromResult((IEnumerable<Movie>)movies);
    }

    private static List<Movie> GetMoviesFromJsonFile(string filePath)
    {
        var content = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<List<Movie>>(content) ?? new List<Movie>();
    }
}
