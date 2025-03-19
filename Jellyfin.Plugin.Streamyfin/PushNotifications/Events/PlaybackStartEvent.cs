using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamyfin.Extensions;
using Jellyfin.Plugin.Streamyfin.PushNotifications.models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events;

/// <summary>
/// Session start notifier.
/// </summary>
public class PlaybackStartEvent(
    ILoggerFactory loggerFactory,
    LocalizationHelper localization,
    IServerApplicationHost applicationHost,
    NotificationHelper notificationHelper
) : BaseEvent(loggerFactory, localization, applicationHost, notificationHelper), IEventConsumer<PlaybackStartEventArgs>
{

    /// <inheritdoc />
    public async Task OnEvent(PlaybackStartEventArgs? eventArgs)
    {
        if (eventArgs == null || Config?.notifications?.PlaybackStarted is not { Enabled: true })
        {
            _logger.LogInformation("PlaybackStartEvent received but currently disabled.");
            return;
        }

        if (eventArgs.Item is null)
        {
            return;
        }

        if (eventArgs.Item.IsThemeMedia)
        {
            // Don't report theme song or local trailer playback.
            return;
        }

        if (eventArgs.Users.Count == 0)
        {
            // No users in playback session.
            return;
        }
        
        CleanupOldEntries();
        
        var notifications = eventArgs.Users
            .Select(user => CreateNotification(user.Username, eventArgs.Item))
            .OfType<ExpoNotificationRequest>()
            .Where(notification => !HasRecentlyProcessed(notification.Body))
            .ToList();

        if (notifications.Count > 0)
        {
            await _notificationHelper.SendToAdmins(
                notifications: notifications, 
                excludedUsersIds: eventArgs.Users.Select(u => u.Id).ToList()
            ).ConfigureAwait(false);
        }
    }

    private ExpoNotificationRequest? CreateNotification(string username, BaseItem item)
    {
        string? name = null;
        var data = new Dictionary<string, object?>();
        List<string> body = [_localization.GetFormatted("UserWatchingNow", args: username)];

        _logger.LogInformation("Creating notification from {0} watching type: {1}", username, item.GetType().Name.Escape());

        switch (item)
        {
            case Movie movie:
                name = _localization.GetFormatted(
                    key: "NameAndYear",
                    args: [movie.Name.Escape(), movie.ProductionYear]
                );
                break;
            case Season season:
                if (!string.IsNullOrEmpty(season.Series.Name) && season.Series?.ProductionYear is not null)
                {
                    name = _localization.GetFormatted(
                        key: "NameAndYear",
                        args: [season.Series.Name.Escape(), season.Series.ProductionYear]
                    );
                }
                else if (!string.IsNullOrEmpty(season.Series?.Name))
                {
                    name = season.Series.Name.Escape();
                }

                if (season.Series?.Id is not null)
                {
                    data["id"] = season.Series.Id.ToString();
                }

                break;
            case Episode episode:

                name = !string.IsNullOrEmpty(episode.Series?.Name) switch
                {
                    // Name + Season + Episode
                    true when episode.Season?.IndexNumber is not null && episode.IndexNumber is not null =>
                        _localization.GetFormatted(
                            key: "SeriesSeasonAndEpisode",
                            args: [
                                episode.Series.Name.Escape(),
                                episode.Season.IndexNumber.Value.ToString(CultureInfo.InvariantCulture),
                                episode.IndexNumber.Value.ToString("00", CultureInfo.InvariantCulture)
                            ]
                        ),
                    // Name + Season
                    true when episode.Season?.IndexNumber is not null => 
                        _localization.GetFormatted(
                            key: "SeriesSeason",
                            args: [
                                episode.Series.Name.Escape(),
                                episode.Season.IndexNumber.Value.ToString(CultureInfo.InvariantCulture)
                            ]
                        ),
                    // Name + Episode
                    true when episode.IndexNumber is not null => 
                        _localization.GetFormatted(
                            key: "SeriesEpisode",
                            args: [
                                episode.Series.Name.Escape(),
                                episode.IndexNumber?.ToString("00", CultureInfo.InvariantCulture) ?? string.Empty
                            ]
                        ),
                    _ => episode.Series?.Name.Escape()
                };

                if (episode.Series?.Id is not null)
                {
                    data["id"] = episode.Series.Id.ToString();
                }

                if (!episode.SeasonId.Equals(Guid.Empty))
                {
                    data["seasonId"] = episode.SeasonId.ToString();
                }

                break;
        }
        
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }
        
        body.Add(name);
        data["type"] = item.GetType().Name.Escape();

        _logger.LogInformation("Sending playback start notification with body: {0}", body);

        return new ExpoNotificationRequest
        {
            Title = _localization.GetString("PlaybackStartTitle"),
            Body = string.Join("\n", body),
            Data = data
        };
    }

    /// <inheritdoc />
    protected override TimeSpan GetRecentEventThreshold()
    {
        if (Config?.notifications?.PlaybackStarted is { RecentEventThreshold: null })
            return base.GetRecentEventThreshold();

        var definedThreshold = (double) Config?.notifications?.PlaybackStarted?.RecentEventThreshold!;
        return TimeSpan.FromSeconds(double.Abs(definedThreshold));
    }
}