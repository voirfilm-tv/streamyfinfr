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
    private readonly ILogger<UserLockedOutEvent>? _logger;
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
    public async Task OnEvent(UserLockedOutEventArgs eventArgs)
    {
        if (_config is { notifications.UserLockedOut.Enabled: false })
        {
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
}