using System.Text.Json.Serialization;

namespace OKEInterviewApp.Models;

public class Movie
{
    [JsonPropertyName("#IMDB_ID")]
    public string Id { get; set; }
    [JsonPropertyName("#TITLE")]
    public string Title { get; set; }
    [JsonPropertyName("#IMG_POSTER")]
    public string Poster { get; set; }
    [JsonPropertyName("#YEAR")]
    public int ReleaseYear { get; set; }
}
