using System.Text;

namespace Nameless;

/// <summary>
///     Core library default values.
/// </summary>
public static class CoreDefaults {
    /// <summary>
    ///     Gets the default encoding (UTF-8 without BOM)
    /// </summary>
    public static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}
