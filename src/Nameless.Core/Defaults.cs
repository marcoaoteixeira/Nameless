using System.Text;

namespace Nameless;

/// <summary>
/// Default values for common types.
/// </summary>
public static class Defaults {
    /// <summary>
    ///     Gets the default encoding (UTF-8 without BOM)
    /// </summary>
    public static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}