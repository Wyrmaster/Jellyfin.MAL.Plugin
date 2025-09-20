using JellyFin.Plugin.Mal.Common;
using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Provider to resolve Anime Series from MyAnimeList
/// </summary>
public class MalSeriesProvider: AbstractBaseMalProvider<Series, SeriesInfo>
{
  #region Constructor
  
  public MalSeriesProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractBaseMalProvider<Series, SeriesInfo>> logger) : base(httpClientFactory, logger)
  {
  }

  #endregion

  #region Overrides

  /// <inheritdoc/>
  public override async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
  {
    Anime? anime = await this.ResolveAnime(info.Path, cancellationToken);

    if (anime == null)
    {
      return new NoMetaDataFound<Series>();
    }
    
    Series series = anime.ToFolder<Series>();

    series.TrySetProviderId(ProviderNames.MyAnimeList, info.GetProviderId(ProviderNames.MyAnimeList));
    
    return new MetadataResult<Series>
    {
      HasMetadata = true,
      Item = series,
      People = await anime.ToPersonInfos(cancellationToken).ToListAsync(cancellationToken),
      Provider = ProviderNames.MyAnimeList,
    };
  }

  #endregion
}