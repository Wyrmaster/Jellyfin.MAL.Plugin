using MediaBrowser.Model.Plugins;

namespace JellyFin.Plugin.Mal;

/// <summary>
///   Configuration for this Jelly fin plugin
/// </summary>
public class PluginConfiguration: BasePluginConfiguration
{
  #region Constructor

  public PluginConfiguration()
  {
    
  }

  #endregion
  
  #region Properties

  /// <summary>
  ///   Hostname of the a custom Jikan endpoint
  /// </summary>
  public string CustomJikanEndPoint { get; set; } = string.Empty;
  
  #endregion
}