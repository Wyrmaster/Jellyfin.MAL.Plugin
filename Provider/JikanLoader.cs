using JikanDotNet;

namespace JellyFin.Plugin.Mal.Provider;

/// <summary>
///   Just a simple singelton instance of the jikan interface as I've not yet figured out a
///   way to distribute it to the different providers using DI lets just see this as a TODO
/// </summary>
public static class JikanLoader
{
  #region Instance

  public static Jikan Instance { get; } = new();

  #endregion
}