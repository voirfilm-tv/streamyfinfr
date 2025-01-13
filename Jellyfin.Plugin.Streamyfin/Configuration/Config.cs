using NJsonSchema.Annotations;

namespace Jellyfin.Plugin.Streamyfin.Configuration;

public class Config
{
  // public Home? home { get; set; }
  [NotNull]
  public Settings.Settings? settings { get; set; }
}
