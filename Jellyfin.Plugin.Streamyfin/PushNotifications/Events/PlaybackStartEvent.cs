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
public class PlaybackStartEvent : EventCache, IEventConsumer<PlaybackStartEventArgs>
{
    private readonly ILogger<PlaybackStartEvent> _logger;
    private readonly IServerApplicationHost _applicationHost;
    private readonly NotificationHelper _notificationHelper;

    public PlaybackStartEvent(
        ILoggerFactory loggerFactory,
        IServerApplicationHost applicationHost,
        NotificationHelper notificationHelper)
    {
        _logger = loggerFactory.CreateLogger<PlaybackStartEvent>();
        _applicationHost = applicationHost;
        _notificationHelper = notificationHelper;
    }

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
        var title = "Playback started";
        var body = $"{username} now watching\n";
        var data = new Dictionary<string, object?>();

        _logger.LogInformation("Creating notification from {0} watching type: {1}", username, item.GetType().Name.Escape());

        switch (item)
        {
            case Movie movie:
                body += $"{movie.Name.Escape()} ({movie.ProductionYear})";
                break;
            case Season season:
                int? year = null;

                if (!string.IsNullOrEmpty(season.Series.Name))
                {
                    title += $"{season.Series.Name.Escape()}";
                }

                if (season.Series?.ProductionYear is not null)
                {
                    title += $" ({season.Series.ProductionYear})";
                }

                if (season.Series?.Id is not null)
                {
                    data["id"] = season.Series.Id.ToString();
                }

                break;
            case Episode episode:
                if (!string.IsNullOrEmpty(episode.Series?.Name))
                {
                    body += $"{episode.Series.Name.Escape()}";
                }

                if (episode.Series?.Id is not null)
                {
                    data["id"] = episode.Series.Id.ToString();
                }

                if (!episode.SeasonId.Equals(Guid.Empty))
                {
                    data["seasonId"] = episode.SeasonId.ToString();
                }

                if (episode.Season?.IndexNumber is not null)
                {
                    body += $", S{episode.Season.IndexNumber.Value.ToString("00", CultureInfo.InvariantCulture)}";
                }

                if (episode.IndexNumber is not null)
                {
                    body += $"E{episode.IndexNumber.Value.ToString("00", CultureInfo.InvariantCulture)}";
                }

                // if (episode.Series?.ProductionYear is not null)
                // {
                //     body += $" ({episode.Series.ProductionYear})";
                // }

                break;
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        data["type"] = item.GetType().Name.Escape();

        _logger.LogInformation("Sending playback start notification with body: {0}", body);

        return new ExpoNotificationRequest
        {
            Title = title,
            Body = body,
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