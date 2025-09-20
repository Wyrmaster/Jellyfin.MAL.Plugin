using MediaBrowser.Controller.Providers;

namespace JellyFin.Plugin.Mal.Common;

/// <summary>
///   Default Response if an Anime cannot be found
/// </summary>
/// <typeparam name="TItemType"></typeparam>
public class NoMetaDataFound<TItemType>: MetadataResult<TItemType>
{
  #region Constructor

  public NoMetaDataFound()
  {
    this.HasMetadata = false;
  }

  #endregion
}