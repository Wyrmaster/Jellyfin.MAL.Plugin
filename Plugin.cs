using JellyFin.Plugin.Mal.Provider;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace JellyFin.Plugin.Mal;

/// <summary>
///   Plugin entry point 
/// </summary>
public class Plugin
  : BasePlugin<PluginConfiguration>,
    IHasWebPages
{
  #region Constructor
  
  public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
  {
    JikanLoader.SetConfiguration(this.Configuration);
  }
  
  #endregion

  #region Override
  
  /// <inheritdoc/>
  public override string Name => "MAL";

  /// <inheritdoc/>
  public override Guid Id => Guid.Parse("f812e84d-bf34-42c3-9a73-1a2c251e38be");

  /// <inheritdoc/>
  public override string Description => "MAL plugin for Jellyfin. Resolving the metadata for Animes using the Jikan API for My Anime List";
  
  #endregion
  
  #region HasWebPages
  
  public IEnumerable<PluginPageInfo> GetPages() =>
  [
    new PluginPageInfo()
    {
      Name = "Mal",
      EmbeddedResourcePath = "JellyFin.Plugin.Mal.Configuration.config.html",
      DisplayName = "Mal",
    }
  ];
  
  #endregion
}