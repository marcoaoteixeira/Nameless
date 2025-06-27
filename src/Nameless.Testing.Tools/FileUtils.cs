using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools;

public static class FileUtils {
    /// <summary>
    ///     Retrieves the resource file full path.
    /// </summary>
    /// <param name="relativePath">The relative path to the resource file.</param>
    /// <returns>
    ///     The full path to the resource file.
    /// </returns>
    /// <remarks>
    ///     This method will look for files inside a folder called "Resources" in
    ///     your current "bin" directory.
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///     If the <paramref name="relativePath"/> do not point to any
    ///     file inside the "Resources" folder.
    /// </exception>
    public static string GetResourceFilePath(string relativePath) {
        var parts = PathHelper.Normalize(relativePath)
                              .Split(Path.DirectorySeparatorChar);

        var result = Assembly.GetCallingAssembly()
                             .GetDirectoryPath(["Resources", .. parts]);

        if (!File.Exists(result)) {
            throw new FileNotFoundException("File not found.", relativePath);
        }

        return result;
    }

    /// <summary>
    ///     Opens a stream for the resource file.
    /// </summary>
    /// <param name="relativePath">The relative path to the resource file.</param>
    /// <returns>
    ///     A <see cref="FileStream"/> representing the resource file.
    /// </returns>
    /// <remarks>
    ///     This method will look for files inside a folder called "Resources" in
    ///     your current "bin" directory.
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///     If the <paramref name="relativePath"/> do not point to any
    ///     file inside the "Resources" folder.
    /// </exception>
    public static FileStream GetResourceFile(string relativePath) {
        var path = GetResourceFilePath(relativePath);

        return File.OpenRead(path);
    }
}
