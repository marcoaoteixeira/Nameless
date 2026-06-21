using System.Diagnostics.CodeAnalysis;

namespace Nameless.ObjectModel;

/// <summary>
/// Represents a Semantic Version as defined by https://semver.org
/// </summary>
public sealed class SemVersion : IEquatable<SemVersion>, IComparable<SemVersion> {
    /// <summary>
    ///     Version 0.0.0
    /// </summary>
    public static SemVersion Empty { get; } = new(0, 0, 0, null, null);

    /// <summary>
    ///     Version 1.0.0 — the canonical first stable release.
    /// </summary>
    public static SemVersion V1 { get; } = new(1, 0, 0, null, null);

    /// <summary>
    ///     Gets the major version. Incremented on incompatible API changes.
    /// </summary>
    public int Major { get; }

    /// <summary>
    ///     Gets the minor version. Incremented on backwards-compatible new
    ///     functionality.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    ///     Gets the patch version. Incremented on backwards-compatible bug
    ///     fixes.
    /// </summary>
    public int Patch { get; }

    /// <summary>
    ///     Gets the pre-release identifier, or <see langword="null"/> if absent
    ///     (e.g. "alpha.1", "rc.2").
    /// </summary>
    public string? PreRelease { get; }

    /// <summary>
    ///     Gets the build metadata, or <see langword="null"/> if absent
    ///     (e.g. "20250610", "sha.abc1234").
    /// </summary>
    public string? Build { get; }

    /// <summary>
    ///     Optional version prefix (<c>'v'</c> or <c>'V'</c>), or
    ///     <see langword="null"/> if absent. The prefix is purely
    ///     cosmetic: it is preserved in <see cref="Format"/> output but
    ///     ignored entirely by equality, hashing, and ordering.
    /// </summary>
    public char? Prefix { get; }

    private SemVersion(int major, int minor, int patch, string? preRelease, string? build, char? prefix = null) {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = string.IsNullOrEmpty(preRelease) ? null : preRelease;
        Build = string.IsNullOrEmpty(build) ? null : build;
        Prefix = prefix;
    }

    /// <summary>
    ///     Parses <paramref name="value"/> into a <see cref="SemVersion"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     When <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">
    ///     When <paramref name="value"/> is not a valid semantic version.
    /// </exception>
    public static SemVersion Parse(string value) {
        Throws.When.NullOrWhiteSpace(value);

        return TryParseInternal(value, out var result)
            ? result
            : throw new FormatException($"'{value}' is not a valid semantic version (semver.org).");
    }

    /// <summary>
    ///     Tries to parse <paramref name="value"/> into
    ///     a <see cref="SemVersion"/>.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> on success;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? value, [NotNullWhen(returnValue: true)] out SemVersion? result) {
        if (!string.IsNullOrWhiteSpace(value)) {
            return TryParseInternal(value, out result);
        }

        result = null;

        return false;
    }

    /// <summary>
    ///     Converts a <see cref="System.Version"/> to a <see cref="SemVersion"/>.
    /// </summary>
    /// <param name="version">
    ///     The <see cref="System.Version"/> instance to convert.
    /// </param>
    /// <param name="prefix">
    ///     Optional version prefix (<c>'v'</c> or <c>'V'</c>).
    /// </param>
    /// <returns>
    ///     A <see cref="SemVersion"/> whose <see cref="Major"/>,
    ///     <see cref="Minor"/>, and <see cref="Patch"/> are mapped from
    ///     <paramref name="version"/>'s <c>Major</c>, <c>Minor</c>, and
    ///     <c>Build</c> components respectively. <see cref="PreRelease"/> and
    ///     <see cref="Build"/> are always <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    ///     <see cref="System.Version"/> uses the component name <c>Build</c>
    ///     for what semver calls <c>Patch</c>; this method accounts for that
    ///     naming mismatch automatically.
    /// </para>
    /// <para>
    ///     Unspecified <c>Minor</c> or <c>Build</c> components
    ///     (value <c>-1</c>) are treated as <c>0</c>.
    /// </para>
    /// <para>
    ///     The <c>Revision</c> component has no semver equivalent. A value
    ///     other than <c>-1</c> or <c>0</c> causes an
    ///     <see cref="ArgumentException"/> because the information would be
    ///     silently lost.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     When <paramref name="version"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     When <paramref name="version"/> has a <c>Revision</c> component
    ///     greater than <c>0</c>, as it cannot be represented in a semantic
    ///     version.
    /// </exception>
    public static SemVersion FromVersion(Version version, char? prefix = null) {
        Throws.When.Null(version);
        Throws.When.GreaterThan(
            version.Revision,
            compare: 0,
            message: $"Cannot convert '{version}' to SemVersion: the Revision component ({version.Revision}) has no semver equivalent."
        );

        return new SemVersion(
            major: version.Major,
            minor: version.Minor == -1 ? 0 : version.Minor,
            patch: version.Build == -1 ? 0 : version.Build,
            preRelease: null,
            build: null,
            prefix: prefix
        );
    }

    private static bool TryParseInternal(string value, [NotNullWhen(returnValue: true)] out SemVersion? result) {
        var match = Infrastructure.RegexCache.SemVersionPattern().Match(value);

        if (!match.Success) {
            result = null;
            return false;
        }

        var prefixGroup = match.Groups["prefix"];

        result = new SemVersion(
            major: int.Parse(match.Groups["major"].Value),
            minor: int.Parse(match.Groups["minor"].Value),
            patch: int.Parse(match.Groups["patch"].Value),
            preRelease: match.Groups["prerelease"].Value,
            build: match.Groups["build"].Value,
            prefix: prefixGroup.Success ? prefixGroup.Value[0] : null
        );

        return true;
    }

    /// <summary>
    ///     Returns the canonical semver string representation,
    ///     e.g. "1.2.3-alpha.1+sha.abc".
    /// </summary>
    /// <param name="includePrefix">
    ///     Whether it should include the prefix, if it is present.
    /// </param>
    /// <returns>
    ///     A string representation of the semantic version.
    /// </returns>
    public string Format(bool includePrefix = false) {
        var version = $"{Major}.{Minor}.{Patch}";

        if (PreRelease is not null) {
            version += $"-{PreRelease}";
        }

        if (Build is not null) {
            version += $"+{Build}";
        }

        return $"{(includePrefix ? Prefix : null)}{version}";
    }

    /// <inheritdoc/>
    public override string ToString() {
        return Format();
    }

    // -------------------------------------------------------------------------
    // Equality (build metadata MUST be ignored per semver spec §10)
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(SemVersion? other) {
        return other is not null &&
               Major == other.Major &&
               Minor == other.Minor &&
               Patch == other.Patch &&
               string.Equals(PreRelease, other.PreRelease, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) {
        return Equals(obj as SemVersion);
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(Major, Minor, Patch, PreRelease);
    }

    // -------------------------------------------------------------------------
    // Comparison (build metadata ignored per spec; pre-release < release)
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    public int CompareTo(SemVersion? other) {
        if (other is null) { return 1; }

        var cmp = Major.CompareTo(other.Major);
        if (cmp != 0) { return cmp; }

        cmp = Minor.CompareTo(other.Minor);
        if (cmp != 0) { return cmp; }

        cmp = Patch.CompareTo(other.Patch);
        if (cmp != 0) { return cmp; }

        return ComparePreRelease(PreRelease, other.PreRelease);
    }

    // semver §11: when major/minor/patch are equal, a pre-release version has
    // lower precedence than the release version; identifiers are compared
    // left-to-right, numerically or lexicographically as appropriate.
    private static int ComparePreRelease(string? left, string? right) {
        if (left is null && right is null) { return 0; }
        
        if (left is null) { return 1; } // release > pre-release
        
        if (right is null) { return -1; }

        var leftParts = left.Split('.');
        var rightParts = right.Split('.');
        var len = Math.Min(leftParts.Length, rightParts.Length);

        for (var idx = 0; idx < len; idx++) {
            var leftIsNum = int.TryParse(leftParts[idx], out var leftNum);
            var rightIsNum = int.TryParse(rightParts[idx], out var rightNum);

            var cmp = (leftIsNum, rightIsNum) switch {
                (true, true) => leftNum.CompareTo(rightNum),
                (true, false) => -1,   // numeric < alphanumeric
                (false, true) => 1,
                _ => string.Compare(leftParts[idx], rightParts[idx], StringComparison.Ordinal)
            };

            if (cmp != 0) { return cmp; }
        }

        return leftParts.Length.CompareTo(rightParts.Length);
    }

    /// <summary>
    ///     Converts a version string representation into
    ///     a <see cref="SemVersion"/> object instance.
    /// </summary>
    /// <param name="value">
    ///     The string version representation.
    /// </param>
    public static implicit operator SemVersion(string value) {
        return Parse(value);
    }
    
    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> and
    ///     <paramref name="right"/> represent the same version. Build
    ///     metadata is ignored per semver spec §10.
    /// </summary>
    /// <remarks>
    ///     Two <see langword="null"/> references are considered equal.
    ///     A <see langword="null"/> and a non-<see langword="null"/>
    ///     reference are never equal.
    /// </remarks>
    public static bool operator ==(SemVersion? left, SemVersion? right) {
        return left?.Equals(right) ?? (right is null);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> and
    ///     <paramref name="right"/> do not represent the same version.
    ///     Build metadata is ignored per semver spec §10.
    /// </summary>
    public static bool operator !=(SemVersion? left, SemVersion? right) {
        return !(left == right);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> has
    ///     lower precedence than <paramref name="right"/> per semver
    ///     spec §11.
    /// </summary>
    /// <remarks>
    ///     A <see langword="null"/> reference is lower than any non-null
    ///     version, and two <see langword="null"/> references are not
    ///     strictly less than each other. Build metadata is ignored.
    /// </remarks>
    public static bool operator <(SemVersion? left, SemVersion? right) {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> has
    ///     higher precedence than <paramref name="right"/> per semver
    ///     spec §11.
    /// </summary>
    /// <remarks>
    ///     A non-null version is higher than a <see langword="null"/>
    ///     reference. Two <see langword="null"/> references are not strictly
    ///     greater than each other. Build metadata is ignored.
    /// </remarks>
    public static bool operator >(SemVersion? left, SemVersion? right) {
        return right is null ? left is not null : right.CompareTo(left) < 0;
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> has
    ///     lower or equal precedence to <paramref name="right"/> per semver
    ///     spec §11. Build metadata is ignored.
    /// </summary>
    public static bool operator <=(SemVersion? left, SemVersion? right) {
        return !(left > right);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="left"/> has
    ///     higher or equal precedence to <paramref name="right"/> per semver
    ///     spec §11. Build metadata is ignored.
    /// </summary>
    public static bool operator >=(SemVersion? left, SemVersion? right) {
        return !(left < right);
    }
}
