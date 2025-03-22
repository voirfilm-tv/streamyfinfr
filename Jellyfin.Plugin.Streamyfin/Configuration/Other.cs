using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Streamyfin.Configuration;

public class Other
{
    [NotNull]
    [Display(Name = "Home page", Description = "The plugin page you want to always load first.")]
    [JsonPropertyName(name: "homePage")]
    public string? HomePage { get; set; }
}