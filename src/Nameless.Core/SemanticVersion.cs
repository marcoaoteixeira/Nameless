using System.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     Represents an immutable Semantic Version (https://semver.org/).
/// </summary>
public readonly record struct SemanticVersion: IComparable<SemanticVersion>, IComparable {
    public int Major { get; init; }
    public int Minor { get; init; }
    public int Patch { get; init; }
    public int? Build { get; init; }

    public SemanticVersion(int major, int minor, int patch, int? build = null) {
        Major = Throws.When.LowerThan(major, compare: 0);
        Minor = Throws.When.LowerThan(minor, compare: 0);
        Patch = Throws.When.LowerThan(patch, compare: 0);

        Build = build is not null
            ? Throws.When.LowerThan(build.Value, compare: 0)
            : null;
    }

    /// <inheritdoc/>
    public int CompareTo(SemanticVersion other) {
        var majorCmp = Major.CompareTo(other.Major);
        if (majorCmp != 0) {
            return majorCmp;
        }

        var minorCmp = Minor.CompareTo(other.Minor);
        if (minorCmp != 0) {
            return minorCmp;
        }

        return Patch.CompareTo(other.Patch);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj) {
        return obj switch {
            null => 1,
            SemanticVersion other => CompareTo(other),
            _ => throw new ArgumentException(
                $"Object must be of type {nameof(SemanticVersion)}.",
                nameof(obj)
            )
        };
    }

    /// <summary>
    ///     Determines whether <paramref name="left"/> has a lower version
    ///     precedence than <paramref name="right"/>.
    /// </summary>
    /// <remarks>
    ///     Build metadata is ignored during comparison, as per the semver
    ///     specification. For structural equality
    ///     (including <see cref="Build"/>), use <c>==</c> instead.
    /// </remarks>
    /// <param name="left">
    ///     The left-hand version.
    /// </param>
    /// <param name="right">
    ///     The right-hand version.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="left"/> has lower
    ///     precedence than <paramref name="right"/>;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator <(SemanticVersion left, SemanticVersion right) {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    ///     Determines whether <paramref name="left"/> has a higher version
    ///     precedence than <paramref name="right"/>.
    /// </summary>
    /// <remarks>
    ///     Build metadata is ignored during comparison, as per the semver
    ///     specification. For structural equality
    ///     (including <see cref="Build"/>), use <c>==</c> instead.
    /// </remarks>
    /// <param name="left">
    ///     The left-hand version.
    /// </param>
    /// <param name="right">
    ///     The right-hand version.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="left"/> has higher
    ///     precedence than <paramref name="right"/>;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator >(SemanticVersion left, SemanticVersion right) {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    ///     Determines whether <paramref name="left"/> has a lower or equal
    ///     version precedence to <paramref name="right"/>.
    /// </summary>
    /// <remarks>
    ///     Build metadata is ignored during comparison, as per the semver
    ///     specification. Two versions with the same <see cref="Major"/>,
    ///     <see cref="Minor"/> and <see cref="Patch"/> are considered equal
    ///     in precedence regardless of their <see cref="Build"/> values.
    ///     For structural equality (including <see cref="Build"/>),
    ///     use <c>==</c> instead.
    /// </remarks>
    /// <param name="left">
    ///     The left-hand version.
    /// </param>
    /// <param name="right">
    ///     The right-hand version.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="left"/> has lower or
    ///     equal precedence to <paramref name="right"/>;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator <=(SemanticVersion left, SemanticVersion right) {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    ///     Determines whether <paramref name="left"/> has a higher or equal
    ///     version precedence to <paramref name="right"/>.
    /// </summary>
    /// <remarks>
    ///     Build metadata is ignored during comparison, as per the semver
    ///     specification. Two versions with the same <see cref="Major"/>,
    ///     <see cref="Minor"/> and <see cref="Patch"/> are considered equal
    ///     in precedence regardless of their <see cref="Build"/> values.
    ///     For structural equality (including <see cref="Build"/>),
    ///     use <c>==</c> instead.
    /// </remarks>
    /// <param name="left">
    ///     The left-hand version.
    /// </param>
    /// <param name="right">
    ///     The right-hand version.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="left"/> has higher or
    ///     equal precedence to <paramref name="right"/>;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator >=(SemanticVersion left, SemanticVersion right) {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    ///     Implicit converts the <paramref name="value"/> into a
    ///     <see cref="SemanticVersion"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The semantic version as string.
    /// </param>
    public static implicit operator SemanticVersion(string value) {
        return Parse(value);
    }

    /// <summary>
    ///     Returns the string representation of this semantic version.
    /// </summary>
    /// <param name="prefix">
    ///     Optional prefix, e.g. "v" → "v1.0.0"
    /// </param>
    /// <param name="suffix">
    ///     Optional pre-release suffix, e.g. "-alpha" → "1.0.0-alpha"
    /// </param>
    /// <param name="includeBuildMetadata">
    ///     When <c>true</c> and <see cref="Build"/> has a value,
    ///     appends "+build.NNN".
    /// </param>
    public string ToString(string? prefix = null, string? suffix = null, bool includeBuildMetadata = false) {
        var buildMetadata = includeBuildMetadata && Build.HasValue
            ? $"+build.{Build}"
            : null;

        return $"{prefix}{Major}.{Minor}.{Patch}{suffix}{buildMetadata}";
    }

    /// <inheritdoc/>
    public override string ToString() {
        return ToString(prefix: null, suffix: null);
    }

    /// <summary>
    ///     Converts a <see cref="System.Version"/> into
    ///     a <see cref="SemanticVersion"/>.
    /// </summary>
    /// <remarks>
    ///     BCL's <see cref="Version"/> layout is
    ///     <c>Major.Minor.Build.Revision</c>, so <c>Build</c> maps to
    ///     <see cref="Patch"/> and <c>Revision</c> maps to
    ///     <see cref="Build"/>. <c>Revision</c> is treated as optional
    ///     (nullable).
    /// </remarks>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public static SemanticVersion FromVersion(Version version) {
        Throws.When.Null(version);

        if (version.Major < 0 || version.Minor < 0) {
            throw new ArgumentException(
                "Version must have explicit Major and Minor components.",
                nameof(version)
            );
        }

        var patch = version.Build >= 0 ? version.Build : 0;
        int? build = version.Revision >= 0 ? version.Revision : null;

        return new SemanticVersion(version.Major, version.Minor, patch, build);
    }

    /// <summary>
    ///     Parses a semantic version string, stripping any leading
    ///     non-numeric prefix (e.g. "v", "ver") and any pre-release suffix
    ///     after the first '-'.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FormatException"/>
    public static SemanticVersion Parse(string value) {
        Throws.When.NullOrWhiteSpace(value);

        return TryParseInternal(value, out var result, out var error)
            ? result.Value
            : throw new FormatException($"Invalid semantic version format: '{value}'. {error}");
    }

    /// <summary>
    ///     Attempts to parse a semantic version string.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="output">
    ///     The <see cref="SemanticVersion"/>, if successfully parsed.
    /// </param>
    /// <returns>
    ///     
    /// </returns>
    public static bool TryParse(string? value, [NotNullWhen(returnValue: true)]out SemanticVersion? output) {
        output = null;

        return !string.IsNullOrWhiteSpace(value) && TryParseInternal(value, out output, out _);
    }

    private static bool TryParseInternal(string value, [NotNullWhen(returnValue: true)] out SemanticVersion? result, out string? error) {
        result = null;
        error = null;

        // Skip any leading non-digit prefix (e.g. "v", "ver", "V")
        var span = value.AsSpan().TrimStart();
        var numStart = 0;

        while (numStart < span.Length && !char.IsDigit(span[numStart])) {
            numStart++;
        }

        if (numStart == span.Length) {
            error = "No numeric version component found.";

            return false;
        }

        var versionSpan = span[numStart..];

        // Isolate build metadata: "+build.XXX" takes precedence over the numeric
        // dot-notation, so we extract it before splitting on '.'
        int? build = null;
        var buildMetaStart = versionSpan.IndexOf('+');

        if (buildMetaStart >= 0) {
            var buildMetaSpan = versionSpan[(buildMetaStart + 1)..]; // "build.456"

            // Expect format "build.NNN"
            if (!buildMetaSpan.StartsWith("build.", StringComparison.OrdinalIgnoreCase)) {
                error = "Build metadata must follow the format '+build.NNN' (e.g. +build.456).";

                return false;
            }

            var buildNumberSpan = buildMetaSpan["build.".Length..];

            if (!TryParseNonNegative(buildNumberSpan, out var buildOutput)) {
                error = "Build metadata number must be a non-negative integer.";

                return false;
            }

            build = buildOutput;
            versionSpan = versionSpan[..buildMetaStart]; // trim "+build.456" from core
        }

        // Strip pre-release suffix — everything from the first '-' onward
        var suffixStart = versionSpan.IndexOf('-');
        var core = suffixStart >= 0
            ? versionSpan[..suffixStart]
            : versionSpan;

        // Split into at most 3 parts now: Major.Minor.Patch  (Build comes from metadata)
        Span<Range> ranges = stackalloc Range[4];
        var count = core.Split(ranges, '.', StringSplitOptions.TrimEntries);

        if (count is < 2 or > 3) {
            error = "Version core must have 2–3 dot-separated components (Major.Minor[.Patch]).";

            return false;
        }

        if (!TryParseNonNegative(core[ranges[0]], out var major) ||
            !TryParseNonNegative(core[ranges[1]], out var minor)) {
            error = "Major and Minor must be non-negative integers.";

            return false;
        }

        int? patch = null;

        if (count == 3) {
            if (!TryParseNonNegative(core[ranges[2]], out var patchOutput)) {
                error = "Patch must be a non-negative integer.";

                return false;
            }

            patch = patchOutput;
        }

        result = new SemanticVersion(major, minor, patch ?? 0, build);

        return true;

        static bool TryParseNonNegative(ReadOnlySpan<char> range, out int output) {
            return int.TryParse(range, out output) && output >= 0;
        }
    }
}
