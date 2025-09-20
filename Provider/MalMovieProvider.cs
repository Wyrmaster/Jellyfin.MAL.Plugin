using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Implementatino for Anime Movies of MyAnimeList
/// </summary>
public class MalMovieProvider: AbstractMalProvider<Movie, MovieInfo>
{
  #region Constructor
  
  public MalMovieProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractMalProvider<Movie, MovieInfo>> logger) : base(httpClientFactory, logger)
  {
  }

  #endregion
}