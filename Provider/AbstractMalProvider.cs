using JellyFin.Plugin.Mal.Common;
using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Base Metadata provider for videos also primary resolves animes by their mal id
/// </summary>
/// <typeparam name="TItemType"></typeparam>
/// <typeparam name="TLookupInfoType"></typeparam>
public abstract class AbstractMalProvider<TItemType, TLookupInfoType>
  : AbstractBaseMalProvider<TItemType, TLookupInfoType>
  where TItemType : Video, IHasLookupInfo<TLookupInfoType>, new()
  where TLookupInfoType : ItemLookupInfo, new()
{
  #region Constructor

  protected AbstractMalProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractBaseMalProvider<TItemType, TLookupInfoType>> logger) : base(httpClientFactory, logger)
  {
  }
  
  #endregion
  
  #region RemoteMetadataProvider

  /// <inheritdoc cref="IRemoteMetadataProvider{TItemType,TLookupInfoType}.GetMetadata"/>
  public override async Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken)
  {
    this.Logger.LogInformation($"Resolving metadata for {info.Path}");
    
    Anime? anime = await this.ResolveAnime(info.Path, cancellationToken);
    if (anime == null)
    {
      // todo implement a way to search for animes
      return new MetadataResult<TItemType>
      {
        HasMetadata = false
      };
    }
      
    MetadataResult<TItemType> metadataResult = new()
    {
      HasMetadata = true,
      Item = anime.ToVideo<TItemType>(),
      Provider = ProviderNames.MyAnimeList,
      People = await anime.ToPersonInfos(cancellationToken: cancellationToken).ToListAsync(cancellationToken: cancellationToken),
      RemoteImages = [
        (
          anime.Images.ResolveTheBestPossibleImage(),
          ImageType.Primary
        )
      ]
    };
      
    return metadataResult;
  }
  
  #endregion
}