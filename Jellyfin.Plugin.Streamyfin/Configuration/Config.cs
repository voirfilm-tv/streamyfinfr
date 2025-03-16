using NJsonSchema.Annotations;

namespace Jellyfin.Plugin.Streamyfin.Configuration;

public class Config
{
  [NotNull]
  public Notifications.Notifications? notifications { get; set; }

  [NotNull]
  public Settings.Settings? settings { get; set; }
}
