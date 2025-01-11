using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Jellyfin.Plugin.Streamyfin.Configuration;


[JsonConverter(typeof(StringEnumConverter))]
public enum DeviceProfile
{
    Expo,
    Native,
    Old
};

[JsonConverter(typeof(StringEnumConverter))]
public enum SearchEngine
{
    Marlin,
    Jellyfin
};

[JsonConverter(typeof(StringEnumConverter))]
public enum DownloadMethod
{
    optimized,
    remux
};

public enum OrientationLock {
    /**
     * The default orientation. On iOS, this will allow all orientations except `Orientation.PORTRAIT_DOWN`.
     * On Android, this lets the system decide the best orientation.
     */
    Default = 0,
    /**
     * Right-side up portrait only.
     */
    PortraitUp = 3,
    /**
     * Left landscape only.
     */
    LandscapeLeft = 6,
    /**
     * Right landscape only.
     */
    LandscapeRight = 7,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum DisplayType
{
    row,
    list
};

[JsonConverter(typeof(StringEnumConverter))]
public enum CardStyle
{
    compact,
    detailed
};

[JsonConverter(typeof(StringEnumConverter))]
public enum ImageStyle
{
    poster,
    cover
};

[JsonConverter(typeof(StringEnumConverter))]
public enum DownloadQuality
{
    Original,
    Low,
    High
}
