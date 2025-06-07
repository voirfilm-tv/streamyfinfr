using System.Globalization;
using Xunit;

namespace Jellyfin.Plugin.Streamyfin.Tests;


/// <summary>
/// Ensure resource file is accessed correctly
/// </summary>
public class LocalizationTests
{
    private LocalizationHelper _helper = new(null, null);
    
    /// <summary>
    /// Test to make sure fallback is english resource
    /// </summary>
    [Fact]
    public void TestFallbackResource()
    {
        Assert.Equal(
            expected: "Playback started",
            actual: _helper.GetString("PlaybackStartTitle", CultureInfo.CreateSpecificCulture("ab-AX"))
        );
    }

    /// <summary>
    /// Strings that don't exist should return the key we used
    /// </summary>
    [Fact]
    public void TestKeyThatDoesNotExist()
    {
        Assert.Equal(
            expected: "ThisStringDoesNotExist",
            actual: _helper.GetString("ThisStringDoesNotExist")
        );
    }
    
    /// <summary>
    /// Test string formats
    /// </summary>
    [Fact]
    public void TestStringFormatLocalization()
    {
        Assert.Equal(
            expected: "Test watching",
            actual: _helper.GetFormatted("UserWatching", args: "Test")
        );
    }
}