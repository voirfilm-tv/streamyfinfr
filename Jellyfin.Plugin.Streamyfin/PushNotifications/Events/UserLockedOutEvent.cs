using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using Jellyfin.Plugin.Streamyfin.Extensions;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Events;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events;

/// <summary>
/// Session start notifier.
/// </summary>
public class UserLockedOutEvent : EventCache, IEventConsumer<UserLockedOutEventArgs>
{
    private readonly ILogger<UserLockedOutEvent> _logger;
    private readonly IServerApplicationHost _applicationHost;
    private readonly NotificationHelper _notificationHelper;

    public UserLockedOutEvent(
        ILoggerFactory loggerFactory,
        IServerApplicationHost applicationHost,
        NotificationHelper notificationHelper)
    {
        _logger = loggerFactory.CreateLogger<UserLockedOutEvent>();
        _applicationHost = applicationHost;
        _notificationHelper = notificationHelper;
    }

    /// <inheritdoc />
    public async Task OnEvent(UserLockedOutEventArgs? eventArgs)
    {
        if (eventArgs?.Argument == null || Config?.notifications?.UserLockedOut is not { Enabled: true })
        {
            _logger.LogInformation("UserLockedOutEvent received but currently disabled.");
            return;
        }

        var notification = new Notification
        {
            Title = "User locked out",
            Body = $"{eventArgs.Argument.Username.Escape()} has been locked out.\nContact admin for reset",
            UserId = eventArgs.Argument.Id
        };

        await _notificationHelper.SendToAdmins(notification).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override TimeSpan GetRecentEventThreshold()
    {
        if (Config?.notifications?.UserLockedOut is { RecentEventThreshold: null })
            return base.GetRecentEventThreshold();

        var definedThreshold = (double) Config?.notifications?.UserLockedOut?.RecentEventThreshold!;
        return TimeSpan.FromSeconds(double.Abs(definedThreshold));
    }
}