using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieApplicationApi.Data;
using MovieApplicationApi.Entities;
using MovieApplicationApi.Exceptions;
using MovieApplicationApi.Interfaces;
using MovieApplicationApi.Models;
using System.Text.Json;

/// <summary>
/// Movie logic.
/// </summary>
public class MovieLogic : IMovieLogic
{
    /// <summary>
    /// HTTP client instance.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Ombdb settings instance.
    /// </summary>
    private readonly OmdbSettings _settings;

    /// <summary>
    /// Database conmtext instance.
    /// </summary>
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Movie logic.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="settings"></param>
    public MovieLogic(HttpClient httpClient, IOptions<OmdbSettings> settings, ApplicationDbContext dbContext)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Search movies by provided title.
    /// </summary>
    /// <param name="title">Movie title.</param>
    public async Task<MovieSearchResponse> SearchMovies(string title)
    {
        var response = await _httpClient.GetAsync($"{_settings.BaseUrl}?s={title}&apikey={_settings.ApiKey}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MovieSearchResponse>(json);

        if (result == null || result.Search == null)
        {
            throw new MovieNotFoundException(title);
        }

        return result;
    }

    /// <summary>
    /// Gets specific movie details by provided imdb id.
    /// </summary>
    /// <param name="imdbId">imdb id.</param>
    public async Task<MovieDetails> GetMovieDetails(string imdbId)
    {
        var response = await _httpClient.GetAsync($"{_settings.BaseUrl}?i={imdbId}&plot=full&apikey={_settings.ApiKey}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MovieDetails>(json);

        if (result == null)
        {
            throw new MovieNotFoundException();
        }

        return result;
    }

    /// <summary>
    /// Gets distinct titles for recent search result.
    /// </summary>
    public async Task<List<string>> GetDistinctTitles()
    {
        var recent = await _dbContext.MovieSearches
            .OrderByDescending(x => x.SearchedAt)
            .ToListAsync();

        var distinctTitles = recent
            .DistinctBy(x => x.Title)
            .Take(5)
            .Select(x => x.Title)
            .ToList();

        return distinctTitles;
    }

    /// <summary>
    /// Saves searches to database.
    /// </summary>
    /// <param name="title">Movie title.</param>
    public async Task SaveSearchesToDb(string title)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            _dbContext.MovieSearches.Add(new MovieSearch { Title = title });
            await _dbContext.SaveChangesAsync();
        }
    }
}
