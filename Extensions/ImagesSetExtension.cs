using JikanDotNet;

namespace JellyFin.Plugin.Mal.Extensions;

/// <summary>
///   Extension methods for Imagesets
/// </summary>
public static class ImagesSetExtension
{
  #region Extension Methods

  /// <summary>
  ///   Resolves the best possible images from the jikan api call
  /// </summary>
  /// <param name="imageSet"></param>
  /// <returns></returns>
  public static string? ResolveTheBestPossibleImage(this ImagesSet? imageSet) =>
    imageSet?.JPG?.MaximumImageUrl
    ?? imageSet?.WebP?.MaximumImageUrl
    ?? imageSet?.JPG?.LargeImageUrl
    ?? imageSet?.WebP?.LargeImageUrl
    ?? imageSet?.JPG?.MediumImageUrl
    ?? imageSet?.WebP?.MediumImageUrl
    ?? imageSet?.JPG?.ImageUrl
    ?? imageSet?.WebP?.ImageUrl
    ?? imageSet?.JPG?.SmallImageUrl
    ?? imageSet?.WebP?.SmallImageUrl;

  #endregion
}