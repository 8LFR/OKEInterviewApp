using OKEInterviewApp.Models;
using Serilog;
using System.Text.Json;

namespace OKEInterviewApp.Repositories;

public class IMDbOTApiMovieRepository : IMovieRepository
{
    private readonly List<Movie> movies;
    private static readonly string url = "https://search.imdbot.workers.dev/?q=The+Godfather";

    public IMDbOTApiMovieRepository()
    {
        movies = GetMoviesFromUrlAsync(url).GetAwaiter().GetResult();
    }

    public Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        return Task.FromResult((IEnumerable<Movie>)movies);
    }

    private static async Task<List<Movie>> GetMoviesFromUrlAsync(string url)
    {
        using var httpClient = new HttpClient();

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Log.Warning($"Failed to fetch movies from URL: {url}. Status code: {response.StatusCode}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

            if (jsonObject.TryGetValue("description", out var description))
            {
                return JsonSerializer.Deserialize<List<Movie>>(description.ToString());
            }
            else
            {
                Log.Warning($"Failed to deserialize data from JSON response for URL: {url}");
                return null;
            }
        }
        catch (JsonException ex)
        {
            Log.Error(ex, $"Failed to deserialize JSON response from URL: {url}");
            return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred while fetching movies from URL: {url}");
            return null;
        }
    }
}
