using JellyFin.Plugin.Mal.Common;
using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using Season = MediaBrowser.Controller.Entities.TV.Season;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Implementation for Anime Seasons
/// </summary>
public class MalSeasonProvider: AbstractBaseMalProvider<Season, SeasonInfo>
{
  #region Constructor

  public MalSeasonProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractBaseMalProvider<Season, SeasonInfo>> logger)
    : base(httpClientFactory, logger)
  {
  }

  #endregion

  #region Overrides

  /// <inheritdoc/>
  public override async Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
  {
    Anime? anime;

    try
    {
      anime = await JikanLoader.GetAnimeByIdAsync(int.Parse(info.SeriesProviderIds[ProviderNames.MyAnimeList]), cancellationToken);
    }
    catch (Exception e)
    {
      this.Logger.LogError(e, $"Error resolving anime");
      anime = null;
    }

    if (anime == null)
    {
      return new NoMetaDataFound<Season>();
    }

    Season season = anime.ToFolder<Season>();
    
    // can be set to one as every anime will have one season and anime "seasons" are all treated as
    // their own shows therefore have their own Ids
    season.IndexNumber = 1;

    return new()
    {
      HasMetadata = true,
      Item = season,
      Provider = ProviderNames.MyAnimeList,
      People = await anime.ToPersonInfos(cancellationToken).ToListAsync(cancellationToken),
    };
  }
  
  #endregion
}