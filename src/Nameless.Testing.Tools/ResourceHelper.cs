using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools;

public static class ResourceHelper {
    /// <summary>
    ///     Retrieves the resource file full path.
    /// </summary>
    /// <param name="relativeFilePath">
    ///     The relative path to the resource file. E.g. "Directory\SubDirectory\File.ext"
    /// </param>
    /// <returns>
    ///     The full path to the resource file.
    /// </returns>
    /// <remarks>
    ///     This method will look for files inside a folder called "Resources" in
    ///     your current "bin" directory. The file path is case-sensitive.
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///     If the <paramref name="relativeFilePath"/> do not point to any
    ///     file inside the "Resources" folder.
    /// </exception>
    public static string GetFilePath(string relativeFilePath) {
        var assembly = Assembly.GetCallingAssembly();

        return GetResourceFilePath(assembly, relativeFilePath);
    }

    /// <summary>
    ///     Opens a stream for the resource file.
    /// </summary>
    /// <param name="relativeFilePath">
    ///     The relative path to the resource file. E.g. "Directory\SubDirectory\File.ext"
    /// </param>
    /// <returns>
    ///     A <see cref="FileStream"/> representing the resource file.
    /// </returns>
    /// <remarks>
    ///     This method will look for files inside a folder called "Resources" in
    ///     your current "bin" directory. The file path is case-sensitive.
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///     If the <paramref name="relativeFilePath"/> do not point to any
    ///     file inside the "Resources" folder.
    /// </exception>
    public static FileStream GetStream(string relativeFilePath) {
        var assembly = Assembly.GetCallingAssembly();
        var filePath = GetResourceFilePath(assembly, relativeFilePath);

        return File.OpenRead(filePath);
    }

    public static string CreateCopy(string relativeFilePath, string? newFileName = null) {
        var assembly = Assembly.GetCallingAssembly();
        var resourceFilePath = GetResourceFilePath(assembly, relativeFilePath);
        var resourceFileName = Path.GetFileNameWithoutExtension(resourceFilePath);
        var resourceFileExtension = Path.GetExtension(resourceFilePath);

        var relativeDirectoryPath = Path.GetDirectoryName(relativeFilePath) ?? string.Empty;
        var temporaryDirectoryPath = GetTemporaryDirectoryPath(assembly, relativeDirectoryPath);

        var fileName = string.IsNullOrWhiteSpace(newFileName)
            ? $"{resourceFileName}{resourceFileExtension}"
            : $"{newFileName}{resourceFileExtension}";

        var destinationFilePath = Path.Combine(temporaryDirectoryPath, fileName);

        File.Copy(resourceFilePath, destinationFilePath, overwrite: true);

        return destinationFilePath;
    }

    private static string GetResourceFilePath(Assembly assembly, string relativeFilePath) {
        relativeFilePath = relativeFilePath.RemoveRoot();

        var filePath = Path.Combine(assembly.GetDirectoryPath(), "Resources", relativeFilePath);

        filePath = PathHelper.Normalize(filePath);

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("File not found.", relativeFilePath);
        }

        return filePath;
    }

    private static string GetTemporaryDirectoryPath(Assembly assembly, string relativeDirectoryPath) {
        var temporaryDirectoryPath = Path.Combine(assembly.GetDirectoryPath(), "Temporary", relativeDirectoryPath.RemoveRoot());

        temporaryDirectoryPath = PathHelper.Normalize(temporaryDirectoryPath);

        return Directory.CreateDirectory(temporaryDirectoryPath).FullName;
    }

    private static string RemoveRoot(this string self) {
        return Path.IsPathRooted(self)
            ? self.RemoveHead([Path.GetPathRoot(self) ?? string.Empty])
            : self;
    }
}
