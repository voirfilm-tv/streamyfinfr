using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum InterruptionLevel
{
    // The system presents the notification immediately, lights up the screen, and can play a sound.
    active,
    // The system presents the notification immediately, lights up the screen, and bypasses the mute switch to play a sound.
    critical,
    // The system adds the notification to the notification list without lighting up the screen or playing a sound.
    passive,
    // The system presents the notification immediately, lights up the screen, can play a sound, and breaks through system notification controls.
    timeSensitive
}