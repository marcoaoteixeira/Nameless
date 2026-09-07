using System.Text.RegularExpressions;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public readonly record struct VersionModel {
    // Anchored regex: major required, minor and status are optional.
    // Status can never appear without a major because the status group is
    // only reachable after a successful major (and optional minor) match.
    private static readonly Regex VersionRegex = new(
        @"^(?<major>\d+)(?:\.(?<minor>\d+))?(?:-(?<status>[a-zA-Z][a-zA-Z0-9]*))?$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture
    );

    public static VersionModel V1 => new(major: 1, minor: null, status: null);

    public int Major { get; }
    public int? Minor { get; }
    public string? Status { get; }

    public VersionModel(int major, int? minor, string? status) {
        Major = major;
        Minor = minor;
        Status = status;
    }

    public string Format() {
        return Major switch {
            > 0 when Minor is null && Status is null => Major.ToString(),
            > 0 when Minor >= 0 && Status is null => $"{Major}.{Minor}",
            > 0 when Minor >= 0 && !string.IsNullOrWhiteSpace(Status) => $"{Major}.{Minor}-{Status}",
            _ => string.Empty
        };
    }

    public static bool TryParse(string? value, out VersionModel output) {
        output = default;
        value ??= string.Empty;

        var match = VersionRegex.Match(value.Trim());

        if (!match.Success) { return false; }

        // 'major' group is always present when the match succeeds.
        var major = int.Parse(match.Groups["major"].Value);
        if (major == 0) { return false; }

        // 'minor' and 'status' are optional — check .Success before reading .Value.
        int? minor = match.Groups["minor"].Success ? int.Parse(match.Groups["minor"].Value) : null;
        var status = match.Groups["status"].Success ? match.Groups["status"].Value : null;

        output = new VersionModel(major, minor, status);

        return true;
    }
}
