using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum Status
{
    ok,
    error
}