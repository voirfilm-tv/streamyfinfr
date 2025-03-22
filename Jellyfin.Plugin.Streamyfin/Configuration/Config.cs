using System.Text.Json.Serialization;
using NJsonSchema.Annotations;

namespace Jellyfin.Plugin.Streamyfin.Configuration;

public class Config
{
  [NotNull]
  public Notifications.Notifications? notifications { get; set; }

  [NotNull]
  public Settings.Settings? settings { get; set; }
  
  [NotNull]
  [JsonPropertyName(name: "other")]
  public Other? Other { get; set; }
}
