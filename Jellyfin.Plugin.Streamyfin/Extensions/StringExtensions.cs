using System;
using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.Streamyfin.Extensions;

public static class StringExtensions
{
    public static string Escape(this string? input) => 
        input?.Replace("\"", "\\\"", StringComparison.Ordinal) ?? string.Empty;

    public static bool IsNullOrNonWord(this string? value) =>
        string.IsNullOrWhiteSpace(value) || Regex.Count(value, "\\w+") == 0;
}