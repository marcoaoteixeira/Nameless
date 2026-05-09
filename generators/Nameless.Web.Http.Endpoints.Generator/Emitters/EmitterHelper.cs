namespace Nameless.Web.Http.Endpoints.Generator.Emitters;

internal static class EmitterHelper {
    // Escapes a value for use inside a C# verbatim-string literal ("...").
    // Replaces backslashes before quotes so the emitted code compiles even when
    // user-provided attribute values contain special characters.
    internal static string EscapeStringLiteral(string? value) {
        return (value ?? string.Empty).Replace("\\", @"\\").Replace("\"", "\\\"");
    }
}
