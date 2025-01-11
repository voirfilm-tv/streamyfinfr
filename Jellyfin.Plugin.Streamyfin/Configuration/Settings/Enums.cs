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
     * All four possible orientations
     */
    All = 1,
    /**
     * Any portrait orientation.
     */
    Portrait = 2,
    /**
     * Right-side up portrait only.
     */
    PortraitUp = 3,
    /**
     * Upside down portrait only.
     */
    PortraitDown = 4,
    /**
     * Any landscape orientation.
     */
    Landscape = 5,
    /**
     * Left landscape only.
     */
    LandscapeLeft = 6,
    /**
     * Right landscape only.
     */
    LandscapeRight = 7,
    /**
     * A platform specific orientation. This is not a valid policy that can be applied in [`lockAsync`](#screenorientationlockasyncorientationlock).
     */
    Other = 8,
    /**
     * An unknown screen orientation lock. This is not a valid policy that can be applied in [`lockAsync`](#screenorientationlockasyncorientationlock).
     */
    Unknown = 9,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum DisplayType
{
    Row,
    List
};

[JsonConverter(typeof(StringEnumConverter))]
public enum CardStyle
{
    Compact,
    Detailed
};

[JsonConverter(typeof(StringEnumConverter))]
public enum ImageStyle
{
    Poster,
    Cover
};

[JsonConverter(typeof(StringEnumConverter))]
public enum DownloadQuality
{
    Original,
    Low,
    High
}
