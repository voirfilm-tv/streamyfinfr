using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamyfin.Configuration;
using Jellyfin.Plugin.Streamyfin.PushNotifications.models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Session;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events;

/// <summary>
/// Session start notifier.
/// </summary>
public class SessionStartEvent : EventCache, IEventConsumer<SessionStartedEventArgs>
{
    private readonly IServerApplicationHost _applicationHost;
    private readonly NotificationHelper _notificationHelper;

    public SessionStartEvent(
        IServerApplicationHost applicationHost,
        NotificationHelper notificationHelper)
    {
        _applicationHost = applicationHost;
        _notificationHelper = notificationHelper;
    }

    /// <inheritdoc />
    public async Task OnEvent(SessionStartedEventArgs eventArgs)
    {
        if (
            eventArgs.Argument is null ||
            _config is { notifications.SessionStarted.Enabled: false }
        )
        {
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
}