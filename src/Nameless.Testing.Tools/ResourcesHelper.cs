using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools;

public static class ResourcesHelper {
    public const string RESOURCES_DIRECTORY_NAME = "Resources";
    public const string TEMPORARY_DIRECTORY_NAME = "VGVtcG9yYXJ5";

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
    public static Stream GetStream(string relativeFilePath) {
        var assembly = Assembly.GetCallingAssembly();
        var filePath = GetResourceFilePath(assembly, relativeFilePath);

        return File.OpenRead(filePath);
    }

    public static string CreateCopy(string relativeFilePath, string? newFileName = null) {
        relativeFilePath = PathHelper.Normalize(relativeFilePath);

        var assembly = Assembly.GetCallingAssembly();
        var resourceFilePath = GetResourceFilePath(assembly, relativeFilePath);

        var relativeDirectoryPath = Path.GetDirectoryName(relativeFilePath) ?? string.Empty;
        var fileName = string.IsNullOrWhiteSpace(newFileName)
            ? Path.GetFileName(relativeFilePath)
            : newFileName;

        // Copy will be created in the temporary folder.
        var directoryPath = Path.Combine(
            GetTemporaryDirectoryPath(assembly),
            relativeDirectoryPath
        );

        // Ensure directory existence
        Directory.CreateDirectory(directoryPath);

        var destinationFilePath = Path.Combine(
            directoryPath,
            fileName
        );

        File.Copy(
            resourceFilePath,
            destinationFilePath,
            overwrite: true
        );

        return destinationFilePath;
    }

    private static string GetResourceFilePath(Assembly assembly, string relativeFilePath) {
        var rootDirectoryPath = Path.Combine(
            assembly.GetDirectoryPath(),
            RESOURCES_DIRECTORY_NAME
        );
        var filePath = Path.IsPathRooted(relativeFilePath)
            ? Path.GetFullPath(relativeFilePath)
            : Path.GetFullPath(relativeFilePath, rootDirectoryPath);

        filePath = PathHelper.Normalize(filePath);

        if (!filePath.StartsWith(rootDirectoryPath)) {
            throw new UnauthorizedAccessException("The specified path is outside the root directory.");
        }

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("Couldn't locate the specified file.", relativeFilePath);
        }

        return filePath;
    }

    private static string GetTemporaryDirectoryPath(Assembly assembly) {
        var directoryPath = Path.Combine(
            assembly.GetDirectoryPath(),
            TEMPORARY_DIRECTORY_NAME
        );

        // Ensure directory existence.
        Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }
}
