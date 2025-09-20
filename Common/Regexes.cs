using System.Text.RegularExpressions;

namespace JellyFin.Plugin.Mal.Common;

/// <summary>
///   General Regular Expressions
/// </summary>
public class Regexes
{
  #region Regexes
  
  /// <summary>
  ///   Detects all boxes e.g. [HorribleSubs]
  /// </summary>
  public static Regex ClearBoxesRegex { get; } = new(@"\[[^\]]+\]", RegexOptions.Compiled);
  
  /// <summary>
  ///   Detects multiple spaces so separators are only every 1 space
  /// </summary>
  public static Regex SpacesRegex { get; } = new(@"\s\s+]", RegexOptions.Compiled);
  
  /// <summary>
  ///   Detects all usual separators
  /// </summary>
  public static Regex SeparatorRegex { get; } = new(@"[\.-_]", RegexOptions.Compiled);
  
  /// <summary>
  ///   detects the quality identifier for a file
  /// </summary>
  public static Regex QualityRegex { get; } = new(@"(10(80|20)|720)(\w+)?", RegexOptions.Compiled);
  
  /// <summary>
  ///   Detects the episode marker for a file
  /// </summary>
  public static Regex SeasonEpisodeRegex { get; } = new(@"(S\s*\d+)?(E\s*\d+)", RegexOptions.Compiled);
  
  /// <summary>
  ///   detect the MAL id in a string
  /// </summary>
  public static Regex MalIdRegex { get; } = new("(?<=\\[mal-?)\\d+(?=\\])",  RegexOptions.Compiled | RegexOptions.IgnoreCase);

  #endregion
}