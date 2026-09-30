using Microsoft.Extensions.FileProviders;

namespace Nameless.IO.Embedded;

internal static class Unsafe {
    internal static string? GetManifestDirectoryName(IDirectoryContents directory) {
        var nameProp = directory.GetType().GetProperty("Name");

        return nameProp?.GetValue(directory) as string;
    }
}