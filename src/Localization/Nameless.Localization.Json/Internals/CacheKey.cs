using System.Diagnostics;

namespace Nameless.Localization.Json;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
internal readonly record struct CacheKey {
    internal string BaseName { get; }

    internal string Location { get; }

    internal string Culture { get; }

    internal string Path => $"{BaseName}.{Location}";

    private string DebuggerDisplayValue
        => $"{Culture}: {BaseName}.{Location}";

    internal CacheKey(string baseName, string location, string culture) {
        BaseName = baseName;
        Location = location;
        Culture = culture;
    }

    internal static CacheKey Create(string baseName, string location, string culture)
        => new(baseName, location, culture);
}