using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Json.Objects;

/// <summary>
/// This class represents a translation, in other words, a file for a culture.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue}")]
public sealed record Translation {
    public static Translation Empty => new(string.Empty, []);

    public string Culture { get; }

    public Region[] Regions { get; }

    private string DebuggerDisplayValue
        => $"Culture: {Culture}";

    public Translation(string culture, Region[] regions) {
        Culture = Prevent.Argument.Null(culture);
        Regions = Prevent.Argument.Null(regions);
    }

    public bool TryGetRegion(string name, [NotNullWhen(returnValue: true)] out Region? output) {
        var current = Regions.SingleOrDefault(item => name == item.Name);

        output = current;

        return current is not null;
    }
}