namespace Jellyfin.Plugin.Streamyfin.Configuration;


public enum DeviceProfile
{
    Expo,
    Native,
    Old
};

public enum SearchEngine
{
    Marlin,
    Jellyfin
};

public enum DownloadMethod
{
    OPTIMIZED,
    REMUX
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

public enum DisplayType
{
    Row,
    List
};

public enum CardStyle
{
    Compact,
    Detailed
};

public enum ImageStyle
{
    Poster,
    Cover
};

public enum DownloadQuality
{
    Original,
    Low,
    High
}
