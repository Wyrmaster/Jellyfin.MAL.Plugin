using JellyFin.Plugin.Mal.Common;
using JellyFin.Plugin.Mal.Extensions;
using JikanDotNet;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Base My anime list provider to provide the necessary metadata for a provider
/// </summary>
/// <typeparam name="TItemType"></typeparam>
/// <typeparam name="TLookupInfoType"></typeparam>
public abstract class AbstractBaseMalProvider<TItemType, TLookupInfoType>
  : IRemoteMetadataProvider<TItemType, TLookupInfoType>
  where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>, new()
  where TLookupInfoType : ItemLookupInfo, new()
{
  #region Fields
  
  protected readonly HttpClient HttpClient;
  protected readonly ILogger<AbstractBaseMalProvider<TItemType, TLookupInfoType>> Logger;

  #endregion

  #region Constructor

  protected AbstractBaseMalProvider(IHttpClientFactory httpClientFactory, ILogger<AbstractBaseMalProvider<TItemType, TLookupInfoType>> logger)
  {
    this.HttpClient = httpClientFactory.CreateClient();
    this.Logger = logger;
  }

  #endregion
  
  #region RemoteMetadataProvider

  /// <inheritdoc cref="IRemoteMetadataProvider{TItemType,TLookupInfoType}.Name"/>
  public virtual string Name => ProviderNames.MyAnimeList;

  /// <inheritdoc cref="IRemoteMetadataProvider{TItemType,TLookupInfoType}.GetSearchResults"/>
  public virtual async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType searchInfo,
    CancellationToken cancellationToken)
  {
    List<RemoteSearchResult> results = new();

    IAsyncEnumerable<Anime> response = JikanLoader.SearchAnimeAsync(searchInfo.Name, cancellationToken);

    await foreach (Anime anime in response)
    {
      results.Add(new RemoteSearchResult
      {
        Name = anime.Titles.FirstOrDefault(entry => entry.Type == Languages.English)?.Title
               ?? anime.Titles.FirstOrDefault(entry => entry.Type == Languages.Default)?.Title
               ?? string.Empty,
        ProductionYear = anime.Aired?.From?.Year,
        ImageUrl = anime.Images.ResolveTheBestPossibleImage(),
        ProviderIds = { [ProviderNames.MyAnimeList] = anime.MalId.ToString() }
      });
    }

    return results;
  }

  /// <inheritdoc cref="IRemoteMetadataProvider{TItemType,TLookupInfoType}.GetMetadata"/>
  public abstract Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken);
  
  /// <inheritdoc/>
  public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    => this.HttpClient.GetAsync(url, cancellationToken);
  
  #endregion
  
  #region Protected Methods

  /// <summary>
  ///   Resolves an Anime from a path of directory/filename
  /// </summary>
  /// <param name="path"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  protected async Task<Anime?> ResolveAnime(string path, CancellationToken cancellationToken = default)
  {
    try
    {
      return (this.TryReadIdFromFile(path) is not { } id
        ? await JikanLoader
            .SearchAnimeAsync(this.ExtractSearchParamFromPath(Path.GetFileNameWithoutExtension(path)), cancellationToken)
            .FirstAsync(cancellationToken: cancellationToken)
        : await JikanLoader.GetAnimeByIdAsync(id, cancellationToken));
    }
    catch (Exception e)
    {
      this.Logger.LogError(e, $"Error while resolving anime: {path}");
      return null;
    }
  } 

  /// <summary>
  ///   Try to load the 
  /// </summary>
  /// <param name="filePath"></param>
  /// <returns></returns>
  protected long? TryReadIdFromFile(string filePath) =>
    Regexes.MalIdRegex.Matches(filePath).LastOrDefault(entry => entry.Success) is not { } match
    || !long.TryParse(match.Value, out long id)
      ? null
      : id;
  
  /// <summary>
  ///   Parse the filename into a search parameter
  /// </summary>
  /// <param name="fileName"></param>
  /// <returns></returns>
  protected string ExtractSearchParamFromPath(string fileName)
  {
    string searchParam = Regexes.ClearBoxesRegex.Replace(fileName, string.Empty);
    searchParam = Regexes.SeparatorRegex.Replace(searchParam, " ");
    searchParam = Regexes.SeasonEpisodeRegex.Replace(searchParam, string.Empty);
    searchParam = Regexes.QualityRegex.Replace(searchParam, string.Empty);
    searchParam = Regexes.SpacesRegex.Replace(searchParam, " ");
    
    return searchParam;
  }

  #endregion
}