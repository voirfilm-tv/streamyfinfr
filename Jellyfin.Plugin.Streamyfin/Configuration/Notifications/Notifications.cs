using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Streamyfin.Configuration.Notifications;


/// <summary>
/// Configuration for a notification
/// </summary>
public class NotificationEnabled
{
    [Display(Name = "Enabled", Description = "if true, the notifications for this event are enabled.")]
    [JsonPropertyName(name: "enabled")]
    public bool Enabled { get; set; }
}

public class NotificationConfig : NotificationEnabled
{
    [Display(Name = "Jellyfin User Ids", Description = "List of jellyfin user ids that this notification is for.")]
    [JsonPropertyName(name: "userIds")]
    public string[] UserIds { get; set; }

    [Display(Name = "Jellyfin Usernames", Description = "List of jellyfin usernames that this notification is for.")]
    [JsonPropertyName(name: "usernames")]
    public string[] Usernames { get; set; }

    [Display(Name = "Forward to admins", Description = "if true, the notification will be forwarded to admins alongside any defined users.")]
    [JsonPropertyName(name: "forwardToAdmins")]
    public bool ForwardToAdmins { get; set; }
}

public class Notifications
{
    [NotNull]
    [Display(Name = "Session Started", Description = "Admins get notified when a jellyfin user is online.")]
    [JsonPropertyName(name: "sessionStarted")]
    public NotificationEnabled? SessionStarted { get; set; }
    
    [NotNull]
    [Display(Name = "Playback Started", Description = "Admins get notified when a jellyfin user is starts playback.")]
    [JsonPropertyName(name: "playbackStarted")]
    public NotificationEnabled? PlaybackStarted { get; set; }
    
    [NotNull]
    [Display(Name = "User locked out", Description = "Admins and locked out user get notified jellyfin locks their account")]
    [JsonPropertyName(name: "userLockedOut")]
    public NotificationEnabled? UserLockedOut { get; set; }
}