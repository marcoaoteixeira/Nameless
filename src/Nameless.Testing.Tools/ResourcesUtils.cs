using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools;

public static class ResourcesUtils {
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
        var currentDirectoryPath = GetCurrentDirectoryPath();
        var parts = PathHelper.Normalize(relativePath)
                              .Split(Path.DirectorySeparatorChar);

        var resourceFilePath = Path.Combine([currentDirectoryPath, "Resources", .. parts]);

        if (!File.Exists(resourceFilePath)) {
            throw new FileNotFoundException("File not found.", relativePath);
        }

        return resourceFilePath;
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

    public static string CreateTemporaryResourceCopy(string resourceRelativePath, string destinationFileName) {
        var resourceFilePath = GetResourceFilePath(resourceRelativePath);

        var temporaryDirectoryPath = GetTemporaryDirectoryPath();
        var destinationParts = PathHelper.Normalize(resourceFilePath)
                                         .Split(Path.DirectorySeparatorChar)
                                         .SkipLast(1);
        var destinationRelativePath = Path.Combine([.. destinationParts, destinationFileName]);

        var destinationFilePath = Path.Combine(temporaryDirectoryPath, destinationRelativePath);

        File.Copy(resourceFilePath, destinationFilePath, overwrite: true);

        return destinationFilePath;
    }

    private static string GetCurrentDirectoryPath() {
        return Assembly.GetCallingAssembly().GetDirectoryPath();
    }

    private static string GetTemporaryDirectoryPath() {
        var currentDirectoryPath = GetCurrentDirectoryPath();
        var temporaryPath = Path.Combine(currentDirectoryPath, "Temporary");

        return Directory.CreateDirectory(temporaryPath).FullName;
    }
}
