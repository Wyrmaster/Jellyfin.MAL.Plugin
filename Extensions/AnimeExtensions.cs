using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Jellyfin.Data.Enums;
using JellyFin.Plugin.Mal.Common;
using JellyFin.Plugin.Mal.Provider;
using JikanDotNet;
using MediaBrowser.Controller.Entities;

namespace JellyFin.Plugin.Mal.Extensions;

/// <summary>
///   Extension Methods for Anime objects
/// </summary>
public static class AnimeExtensions
{
  #region Fields

  private static readonly Regex DurationRegex = new(@"((\d+)\s*?hr)?\s?((\d+)\s*?min)");

  #endregion
  
  #region Extention Methods

  /// <summary>
  ///   Converts an Animeobject to a TVideo
  /// </summary>
  /// <param name="anime"></param>
  /// <typeparam name="TVideo"></typeparam>
  /// <returns></returns>
  public static TVideo ToVideo<TVideo>(this Anime anime) where TVideo : Video, new() =>
    new()
    {
      Name = anime.Titles.FirstOrDefault(entry => entry.Type == Languages.Default)?.Title 
             ?? anime.Titles.FirstOrDefault(entry => entry.Type == Languages.English)?.Title
             ?? string.Empty,
      OriginalTitle = anime.Titles.FirstOrDefault(entry => entry.Type == Languages.Default)?.Title 
                      ?? string.Empty,
      Overview = anime.Synopsis,
      ProductionYear = anime.Aired.From?.Year ?? 0,
      PremiereDate = anime.Aired.From,
      EndDate = anime.Aired.To,
      CommunityRating = (float) (anime.Score ?? 0.0f),
      RunTimeTicks = GetDuration(anime.Duration).Ticks,
      Genres = anime.Genres.Select(entry => entry.Name).ToArray(),
      Studios = anime.Studios.Select(entry => entry.Name).ToArray(),
      ProviderIds = new Dictionary<string, string>{{ProviderNames.MyAnimeList, anime.MalId?.ToString() ?? "0"}}
    };
  
  /// <summary>
  ///   convert an anime to a folder item
  /// </summary>
  /// <param name="anime"></param>
  /// <typeparam name="TItem"></typeparam>
  /// <returns></returns>
  public static TItem ToFolder<TItem>(this Anime anime) where TItem : Folder, new() =>
    new()
    {
      Name = anime.Titles.FirstOrDefault(entry => entry.Type == Languages.Default)?.Title 
             ?? anime.Titles.FirstOrDefault(entry => entry.Type == Languages.English)?.Title
             ?? string.Empty,
      OriginalTitle = anime.Titles.FirstOrDefault(entry => entry.Type == Languages.English)?.Title 
                      ?? string.Empty,
      Overview = anime.Synopsis,
      ProductionYear = anime.Aired.From?.Year ?? 0,
      PremiereDate = anime.Aired.From,
      EndDate = anime.Aired.To,
      CommunityRating = (float) (anime.Score ?? 0.0f),
      RunTimeTicks = GetDuration(anime.Duration).Ticks,
      Genres = anime.Genres.Select(entry => entry.Name).ToArray(),
      Studios = anime.Studios.Select(entry => entry.Name).ToArray(),
      ProviderIds = new Dictionary<string, string>{{ProviderNames.MyAnimeList, anime.MalId?.ToString() ?? "0"}}
    };

  /// <summary>
  ///   Read the characters of an anime 
  /// </summary>
  /// <param name="anime"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async IAsyncEnumerable<PersonInfo> ToPersonInfos(this Anime anime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (anime.MalId == null)
    {
      yield break;
    }

    await foreach (IAsyncGrouping<string, AnimeCharacter> characters in JikanLoader.GetAnimeCharactersAsync(anime.MalId.Value, cancellationToken)
                     .Where(entry => entry.VoiceActors.Any(actorEntry => actorEntry.Language == Languages.Japanese))
                     .GroupBy(entry => entry.VoiceActors.First(actorEntry => actorEntry.Language == Languages.Japanese).Person.Name).WithCancellation(cancellationToken))
    {
      VoiceActorEntry voiceActor = (await characters.FirstAsync(cancellationToken: cancellationToken)).VoiceActors.First(entry => entry.Language == Languages.Japanese);

      yield return new()
      {
        Name = characters.Key,
        Role = string.Join("/",characters.Select(entry => entry.Character.Name)),
        Type = PersonKind.Actor,
        ImageUrl = voiceActor.Person.Images.ResolveTheBestPossibleImage(),
        ProviderIds = { [ProviderNames.MyAnimeList] = voiceActor.Person.MalId.ToString() }
      };
    }
  }
  
  #endregion

  #region Private Methods

  /// <summary>
  ///   Try to parse a 1 hr 49 min to a timespan
  /// </summary>
  /// <param name="duration"></param>
  /// <returns></returns>
  private static TimeSpan GetDuration(string duration)
  {
    if (DurationRegex.Match(duration) is not { Success: true } match)
    {
      return TimeSpan.Zero;
    }
    
    return TimeSpan.Zero
      .Add(TimeSpan.FromHours(match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0))
      .Add(TimeSpan.FromMinutes(match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0))
    ;
  }

  #endregion
}