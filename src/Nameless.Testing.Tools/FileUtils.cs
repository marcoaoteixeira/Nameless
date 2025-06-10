using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools;

public static class FileUtils {
    public static string GetResourceFilePath(string relativePath) {
        var parts = PathHelper.Normalize(relativePath)
                              .Split(Path.DirectorySeparatorChar);

        return Assembly.GetCallingAssembly()
                       .GetDirectoryPath(["Resources", .. parts]);
    }

    public static FileStream GetResourceFile(string relativePath) {
        var path = GetResourceFilePath(relativePath);

        return File.OpenRead(path);
    }
}
