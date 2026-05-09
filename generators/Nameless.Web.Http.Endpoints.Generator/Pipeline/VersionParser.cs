namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class VersionParser {
    internal static bool TryParse(string value, out int major, out int minor, out int patch) {
        major = minor = patch = 0;

        if (string.IsNullOrWhiteSpace(value)) {
            return false;
        }

        var parts = value.Split('.');
        if (parts.Length is 0 or > 3) {
            return false;
        }

        if (!int.TryParse(parts[0], out major) || major < 0) {
            return false;
        }

        switch (parts.Length) {
            case >= 2 when !int.TryParse(parts[1], out minor) || minor < 0:
            case 3 when !int.TryParse(parts[2], out patch) || patch < 0:
                return false;
            default:
                return true;
        }
    }

    internal static string Format(int major, int minor, int patch) {
        return patch == 0 ? $"{major}.{minor}" : $"{major}.{minor}.{patch}";
    }
}
