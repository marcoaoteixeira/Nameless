namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class VersionParser {
    internal static bool TryParse(string value, out int major, out int minor) {
        major = 0;
        minor = 0;

        if (string.IsNullOrWhiteSpace(value)) { return false; }

        var parts = value.Split('.');
        if (parts.Length is 0 or > 2) { return false;}

        _ = int.TryParse(parts[0], out major);

        if (parts.Length == 2) {
            return int.TryParse(parts[1], out minor) && minor >= 0;
        }

        return major > 0 && minor >= 0;
    }

    internal static string Format(int major, int minor) {
        return minor == 0 ? $"{major}" : $"{major}.{minor}";
    }
}
