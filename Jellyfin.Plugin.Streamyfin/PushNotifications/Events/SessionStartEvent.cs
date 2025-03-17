using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamyfin.PushNotifications.models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Session;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events;

/// <summary>
/// Session start notifier.
/// </summary>
public class SessionStartEvent : EventCache, IEventConsumer<SessionStartedEventArgs>
{
    private readonly ILogger<SessionStartEvent> _logger;
    private readonly IServerApplicationHost _applicationHost;
    private readonly NotificationHelper _notificationHelper;

    public SessionStartEvent(
        ILoggerFactory loggerFactory,
        IServerApplicationHost applicationHost,
        NotificationHelper notificationHelper)
    {
        _logger = loggerFactory.CreateLogger<SessionStartEvent>();
        _applicationHost = applicationHost;
        _notificationHelper = notificationHelper;
    }

    /// <inheritdoc />
    public async Task OnEvent(SessionStartedEventArgs? eventArgs)
    {
        if (eventArgs?.Argument == null || Config?.notifications?.SessionStarted is not { Enabled: true })
        {
            _logger.LogInformation("SessionStartEvent received but currently disabled.");
            return;
        }

        // Clean up old session entries when a new session event is triggered
        CleanupOldEntries();

        // Prevent the same notification per device
        string sessionKey = eventArgs.Argument.DeviceId;

        // Check if we've processed a similar event recently
        if (HasRecentlyProcessed(sessionKey))
        {
            return;
        }
        
        List<ExpoNotificationRequest> notifications = [
            new()
            {
                Title = $"Session started",
                Body = $"{eventArgs.Argument.UserName} is now online"
            }
        ];

        await _notificationHelper
            .SendToAdmins(
                notifications: notifications,
                excludedUsersIds: [eventArgs.Argument.UserId]
            ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override TimeSpan GetRecentEventThreshold()
    {
        if (Config?.notifications?.SessionStarted is { RecentEventThreshold: null })
            return base.GetRecentEventThreshold();

        var definedThreshold = (double) Config?.notifications?.SessionStarted?.RecentEventThreshold!;
        return TimeSpan.FromSeconds(double.Abs(definedThreshold));
    }
}