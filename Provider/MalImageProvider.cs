using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using Season = MediaBrowser.Controller.Entities.TV.Season;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Provider for resolving remote images from MAL
/// </summary>
public class MalImageProvider: IRemoteImageProvider
{
  #region Field

  private readonly ILogger<MalImageProvider> _logger;
  private readonly HttpClient _httpClient;

  #endregion

  #region Constructor

  public MalImageProvider(ILogger<MalImageProvider> logger,IHttpClientFactory httpClientFactory)
  {
    this._logger = logger;
    this._httpClient =  httpClientFactory.CreateClient(nameof(MalImageProvider));
  }

  #endregion
  
  #region RemoteImageProvider

  /// <inheritdoc cref="IRemoteImageProvider.Name"/>
  public string Name => ProviderNames.MyAnimeList;

  /// <inheritdoc cref="IRemoteImageProvider.Supports"/>
  public bool Supports(BaseItem item) => item is Video or Series or Season;

  /// <inheritdoc cref="IRemoteImageProvider.GetSupportedImages"/>
  public IEnumerable<ImageType> GetSupportedImages(BaseItem item) =>
  [
    ImageType.Primary
  ];

  /// <inheritdoc cref="IRemoteImageProvider.GetImages"/>
  public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
  {
    if (!item.ProviderIds.ContainsKey(ProviderNames.MyAnimeList)
        || !long.TryParse(item.ProviderIds[ProviderNames.MyAnimeList], out long id))
    {
      return [];
    }

    try
    {
      Anime anime = await JikanLoader.GetAnimeByIdAsync(id, cancellationToken);

      return
      [
        new()
        {
          Url = anime.Images.ResolveTheBestPossibleImage(),
          Type = ImageType.Primary,
          ProviderName = ProviderNames.MyAnimeList,
        }
      ];
    }
    catch (Exception e)
    {
      this._logger.LogError(e,$"Unable to resolve images for file: {item.Path}");
      return [];
    }
  }

  /// <inheritdoc cref="IRemoteImageProvider.GetImageResponse"/>
  public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    => this._httpClient.GetAsync(url, cancellationToken);

  #endregion
}