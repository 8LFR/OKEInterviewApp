using Microsoft.Extensions.Caching.Memory;
using Moq;
using OKEInterviewApp.Models;
using OKEInterviewApp.Repositories;
using OKEInterviewApp.Services;

namespace OKEInterviewApp.Tests;

public class MovieServiceTests
{
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ICacheEntry> _mockCacheEntry;
    private readonly Mock<IMovieRepository> _mockStaticFileMovieRepository;
    private readonly Mock<IMovieRepository> _mockIMDbOTApiMovieRepository;
    private readonly MovieService _movieService;

    public MovieServiceTests()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockCacheEntry = new Mock<ICacheEntry>();
        _mockStaticFileMovieRepository = new Mock<IMovieRepository>();
        _mockIMDbOTApiMovieRepository = new Mock<IMovieRepository>();

        _mockStaticFileMovieRepository.Setup(repo => repo.GetMoviesAsync()).ReturnsAsync(new List<Movie>
            {
                new() { Id = "1", Title = "Static Movie 1" },
                new() { Id = "2", Title = "Static Movie 2" }
            });

        _mockIMDbOTApiMovieRepository.Setup(repo => repo.GetMoviesAsync()).ReturnsAsync(new List<Movie>
            {
                new() { Id = "1", Title = "IMDB Movie 1" },
                new() { Id = "2", Title = "IMDB Movie 2" }
            });

        string? keyPayload = null;
        _mockCache
            .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
            .Callback((object k) => keyPayload = (string)k)
            .Returns(_mockCacheEntry.Object);

        _movieService = new MovieService(new[] { _mockStaticFileMovieRepository.Object, _mockIMDbOTApiMovieRepository.Object }, _mockCache.Object);
    }

    [Fact]
    public async Task GetMoviesAsync_ReturnsResult_WhenCalled()
    {
        // Arrange
        var staticSource = _mockStaticFileMovieRepository.GetType().GetGenericArguments().First().Name;
        var imdbSource = _mockIMDbOTApiMovieRepository.GetType().GetGenericArguments().First().Name;

        // Act
        var staticResult = await _movieService.GetMoviesAsync(staticSource);
        var imdbResult = await _movieService.GetMoviesAsync(imdbSource);

        // Assert
        Assert.Equal(2, staticResult.Count());
        Assert.Equal(2, imdbResult.Count());
    }

    [Fact]
    public async Task GetMoviesAsync_ReturnsCachedResult_WhenCalledTwice()
    {
        // Arrange
        var staticSource = _mockStaticFileMovieRepository.GetType().GetGenericArguments().First().Name;
        var imdbSource = _mockIMDbOTApiMovieRepository.GetType().GetGenericArguments().First().Name;

        // Act
        var staticResult1 = await _movieService.GetMoviesAsync(staticSource);
        var imdbResult1 = await _movieService.GetMoviesAsync(imdbSource);
        var staticResult2 = await _movieService.GetMoviesAsync(staticSource);
        var imdbResult2 = await _movieService.GetMoviesAsync(imdbSource);

        // Assert
        Assert.Equal(staticResult1, staticResult2);
        Assert.Equal(imdbResult1, imdbResult2);
    }

    [Fact]
    public async Task GetMoviesAsync_ThrowsArgumentException_WhenRepositoryNotFound()
    {
        // Arrange
        var nonExistingSource = "NonExistingRepository";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _movieService.GetMoviesAsync(nonExistingSource));
        Assert.Equal($"Repository for source '{nonExistingSource}' not found.", exception.Message);
    }
}
