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
  ///   Maximum number of characters to show
  /// </summary>
  public int MaxCharactersToShow { get; set; } = 250;

  #endregion
}