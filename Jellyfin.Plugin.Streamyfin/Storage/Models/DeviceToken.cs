using System;
using Newtonsoft.Json;

namespace Jellyfin.Plugin.Streamyfin.Storage.Models;

public class DeviceToken
{
    [JsonProperty(PropertyName = "token")]
    public string Token { get; set; }
    [JsonProperty(PropertyName = "deviceId")]
    public Guid DeviceId { get; set; }
    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; }
    [JsonProperty(PropertyName = "timestamp")]
    public long Timestamp { get; set; }
}