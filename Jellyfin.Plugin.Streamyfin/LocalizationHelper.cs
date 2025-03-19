#pragma warning disable CA1869

using System.Globalization;
using System.Resources;


namespace Jellyfin.Plugin.Streamyfin;

/// <summary>
/// Serialization settings for json and yaml
/// </summary>
public class LocalizationHelper
{
    private ResourceManager _resourceManager;

    public LocalizationHelper()
    {
        _resourceManager = new ResourceManager(
            baseName: "Jellyfin.Plugin.Streamyfin.Resources.Strings",
            assembly: typeof(LocalizationHelper).Assembly
        );
    }

    /// <summary>
    /// Get string resource or fallback to key to avoid nullable strings
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public string GetString(string key, CultureInfo? cultureInfo = null) => 
        _resourceManager.GetString(key, cultureInfo) ?? key;

    /// <summary>
    /// Get a string resource that requires string formatting
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cultureInfo"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetFormatted(string key, CultureInfo? cultureInfo = null, params object[] args) {
        var resource = _resourceManager.GetString(key, cultureInfo);
        return resource == null ? key : string.Format(cultureInfo, resource, args);
    }
}