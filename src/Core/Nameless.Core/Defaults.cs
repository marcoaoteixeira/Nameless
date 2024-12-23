using System.Text;

namespace Nameless;

internal static class Defaults {
    /// <summary>
    /// Gets the default encoding (UTF-8 without BOM)
    /// </summary>
    internal static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}
