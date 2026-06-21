namespace Nameless.Web.Generators.Infrastructure;

public static class StringHelpers {
    // Escapes a value for use inside a C# verbatim-string literal ("...").
    // Replaces backslashes before quotes so the emitted code compiles even when
    // user-provided attribute values contain special characters.
    public static string EscapeStringLiteral(string? value) {
        return (value ?? string.Empty).Replace("\\", @"\\").Replace("\"", "\\\"");
    }

    // Converts an arbitrary string into a valid C# identifier fragment by replacing
    // non-alphanumeric characters with underscores.
    public static string Sanitize(string value) {
        return new string(
            [.. value.Select(static @char => char.IsLetterOrDigit(@char) ? @char : '_')]
        );
    }
}
