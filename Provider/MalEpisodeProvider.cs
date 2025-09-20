using System.Text.RegularExpressions;
using JikanDotNet;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Provider to resolve Anime Episodes from MyAnimeList
/// </summary>
public class MalEpisodeProvider: AbstractMalProvider<Episode, EpisodeInfo>
{
  #region Fields

  private static readonly Regex DetectEpisodeNumber = new(@"-\s(S\d+E(\d+)|E(\d+)|(\d+))", RegexOptions.Compiled);

  #endregion
  
  #region Constructor
  
  public MalEpisodeProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractMalProvider<Episode, EpisodeInfo>> logger) : base(httpClientFactory, logger)
  {
  }

  #endregion

  #region Overrides

  public override async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
  {
    MetadataResult<Episode> result = (await base.GetMetadata(info, cancellationToken));

    if (!result.Item.ProviderIds.ContainsKey(ProviderNames.MyAnimeList)
        || !long.TryParse(result.Item.ProviderIds[ProviderNames.MyAnimeList], out long id))
    {
      return result;
    }

    Match match = DetectEpisodeNumber.Match(Path.GetFileNameWithoutExtension(info.Path));
    if (match.Success && int.TryParse(match.Groups[2].Success
          ? match.Groups[2].Value
          : match.Groups[3].Success
            ? match.Groups[3].Value
            : match.Groups[4].Success
              ? match.Groups[4].Value
              : "0", out int idx))
    {
      AnimeEpisode episode = (await JikanLoader.Instance.GetAnimeEpisodeAsync(id, idx, cancellationToken)).Data;
      result.Item.Name = episode.Title;
      // season number can be set to 1 as animes don't really have seasons in that sense.
      // so for the mal implementation every season has its own id
      result.Item.ParentIndexNumber = 1;
      result.Item.OriginalTitle = episode.TitleRomanji;
      result.Item.Overview = episode.Synopsis;
      
      result.Item.IndexNumber = idx;
    }

    return result;
  }

  #endregion
}