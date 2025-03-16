using System;

namespace Jellyfin.Plugin.Streamyfin.Extensions;

public static class StringExtensions
{
    public static string Escape(this string? input) => 
        input?.Replace("\"", "\\\"", StringComparison.Ordinal) ?? string.Empty;
}