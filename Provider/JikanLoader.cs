using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using JikanDotNet;
using JikanDotNet.Config;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Just a simple singelton instance of the jikan interface as I've not yet figured out a
///   way to distribute it to the different providers using DI lets just see this as a TODO
/// </summary>
public static class JikanLoader
{
  #region Fields

  private static readonly ConcurrentDictionary<long, Anime> AnimeDict = new();
  private static readonly ConcurrentDictionary<long, IEnumerable<AnimeCharacter>> AnimeCharacterDict = new();
  private static readonly ConcurrentDictionary<string, AnimeEpisode> AnimeEpisodeDict = new();
  private static readonly ConcurrentDictionary<string, IEnumerable<Anime>> AnimeSearchDict = new();

  private static string _url = string.Empty;
  private static PluginConfiguration? _configuration;

  #endregion
  
  #region Instance

  private static Jikan? _instance;

  /// <summary>
  ///   Static Singleton reference to the Jikan API
  /// </summary>
  private static Jikan Instance
  {
    get
    {
      // no configuration return default
      if (_configuration == null)
      {
        return _instance ??= new();
      }
      
      // if custom endpoint is null or empty then we can assume that we need a default
      if (string.IsNullOrEmpty(_configuration.CustomJikanEndPoint))
      {
        // if the old is also null or empty then we create the default
        if (string.IsNullOrEmpty(_url))
        {
          return _instance ??= new();
        }
      }

      // otherwise compare the previously used ones and create one with a custom endpoint
      if (_configuration.CustomJikanEndPoint != _url)
      {
        _configuration.CustomJikanEndPoint = _configuration.CustomJikanEndPoint;
        _instance = new(new JikanClientConfiguration(), new HttpClient
        {
          BaseAddress = new Uri(_configuration.CustomJikanEndPoint),
        });
      }

      return _instance ??= new();
    }
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Load an anime by its id
  /// </summary>
  /// <param name="id">MAL id of an Anime</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async Task<Anime> GetAnimeByIdAsync(long id, CancellationToken cancellationToken = default)
  {
    Anime? anime;
    if (AnimeDict.TryGetValue(id, out anime))
    {
      return anime;
    }
    anime = (await Instance.GetAnimeAsync(id, cancellationToken)).Data;

    AnimeDict[id] = anime;
    
    return anime;
  }

  /// <summary>
  ///   Try to find the first occurrence of an anime that matches
  /// </summary>
  /// <param name="name">name of an anime to resolve</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async IAsyncEnumerable<Anime> SearchAnimeAsync(string name, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (AnimeSearchDict.TryGetValue(name, out IEnumerable<Anime>? animes))
    {
      foreach (Anime anime in animes)
      {
        yield return anime;
      }
      yield break;
    }
    
    AnimeSearchDict[name] = (await Instance.SearchAnimeAsync(name, cancellationToken)).Data;
    
    foreach (Anime anime in AnimeSearchDict[name])
    {
      yield return anime;
    }
  }
  
  /// <summary>
  ///   Resolves an anime episode by its mal id and the episode index
  /// </summary>
  /// <param name="malId">id of an anime to resolve</param>
  /// <param name="episodeIndex">index of the episode to resolve</param>
  /// <param name="cancellationToken"></param>
  public static async Task<AnimeEpisode> GetAnimeEpisodeAsync(long malId, int episodeIndex, CancellationToken cancellationToken)
  {
    string key = $"{malId}.E{episodeIndex}";

    AnimeEpisode? animeEpisode;
    
    if (AnimeEpisodeDict.TryGetValue(key, out animeEpisode))
    {
      return animeEpisode;
    }
    
    animeEpisode = (await Instance.GetAnimeEpisodeAsync(malId, episodeIndex, cancellationToken)).Data;

    AnimeEpisodeDict[key] = animeEpisode;
    
    return animeEpisode;
  }
  
  /// <summary>
  ///   Resolve all Voice actors for a given Anime
  /// </summary>
  /// <param name="malId">id of an anime </param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async IAsyncEnumerable<AnimeCharacter> GetAnimeCharactersAsync(long malId, [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    if (AnimeCharacterDict.TryGetValue(malId, out IEnumerable<AnimeCharacter>? characters))
    {
      foreach (AnimeCharacter animeCharacter in characters)
      {
        yield return animeCharacter;
      }
      yield break;
    }
    
    AnimeCharacterDict[malId] = (await Instance.GetAnimeCharactersAsync(malId, cancellationToken)).Data;
    
    foreach (AnimeCharacter animeCharacter in AnimeCharacterDict[malId])
    {
      yield return animeCharacter;
    }
  }

  /// <summary>
  ///   Sets the configuration for the custom jikan api
  /// </summary>
  /// <param name="configuration"></param>
  public static void SetConfiguration(PluginConfiguration configuration)
  {
    _configuration = configuration;
  }

  #endregion
}