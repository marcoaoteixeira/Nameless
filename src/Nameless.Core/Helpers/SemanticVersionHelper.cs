using System.Diagnostics.CodeAnalysis;

namespace Nameless.Helpers;

public static class SemanticVersionHelper {
    public static bool TryParse(string value, [NotNullWhen(returnValue: true)] out Version? output) {
        output = null;

        // do not accept null, empty or only whitespace.
        if (string.IsNullOrWhiteSpace(value)) { return false; }

        // match the correct semantic version regexp
        var match = RegexCache.SemanticVersionPattern().Match(value);
        
        if (!match.Success) { return false; }

        output = new Version(match.Groups[1].Value);

        return true;
    }
}
