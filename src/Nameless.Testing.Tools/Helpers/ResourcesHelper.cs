using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools.Helpers;

public static class ResourcesHelper {
    public const string ROOT_DIRECTORY_NAME = "Resources";
    public const string TEMPORARY_DIRECTORY_NAME = "__tmp__";

    /// <summary>
    ///     Retrieves the resource file full path.
    /// </summary>
    /// <param name="relativeFilePath">
    ///     The relative path to the resource file. E.g. "Directory\SubDirectory\File.ext"
    /// </param>
    /// <param name="ensureFileExistence">
    ///     Whether to ensure the file existence or not.
    ///     Defaults to <c>true</c>.
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
    public static string GetFilePath(string relativeFilePath, bool ensureFileExistence = true) {
        var assembly = Assembly.GetCallingAssembly();

        return InnerGetResourceFilePath(assembly, relativeFilePath, ensureFileExistence);
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
        var filePath = InnerGetResourceFilePath(assembly, relativeFilePath);

        return File.OpenRead(filePath);
    }

    public static string CreateCopy(string relativeFilePath, string? newFileName = null) {
        var assembly = Assembly.GetCallingAssembly();
        var resourceFilePath = InnerGetResourceFilePath(
            assembly,
            relativeFilePath
        );

        var relativeDirectoryPath = Path.GetDirectoryName(relativeFilePath) ?? string.Empty;
        var fileExtension = Path.GetExtension(resourceFilePath);

        newFileName = string.IsNullOrWhiteSpace(newFileName)
            ? $"{Guid.CreateVersion7():N}{fileExtension}"
            : Path.GetFileName(newFileName);

        var temporaryDirectoryPath = InnerGetTemporaryDirectoryPath(assembly);
        var temporaryFilePath = Path.Combine(
            temporaryDirectoryPath,
            relativeDirectoryPath,
            newFileName
        );
        
        File.Copy(
            resourceFilePath,
            temporaryFilePath,
            overwrite: true
        );

        return temporaryFilePath;
    }

    private static string InnerGetResourceFilePath(Assembly assembly, string relativeFilePath, bool ensureFileExistence = true) {
        var root = InnerGetRootDirectoryPath(assembly);

        var filePath = Path.IsPathRooted(relativeFilePath)
            ? Path.GetFullPath(relativeFilePath)
            : Path.GetFullPath(relativeFilePath, root);

        filePath = PathHelper.Normalize(filePath);

        if (!filePath.StartsWith(root)) {
            throw new UnauthorizedAccessException("The specified path is outside the root directory.");
        }

        if (ensureFileExistence && !File.Exists(filePath)) {
            throw new FileNotFoundException("Couldn't locate the specified file.", relativeFilePath);
        }

        return filePath;
    }

    private static string InnerGetRootDirectoryPath(Assembly assembly) {
        var path = Path.Combine(assembly.GetDirectoryPath(), ROOT_DIRECTORY_NAME);

        // Ensure directory existence.
        Directory.CreateDirectory(path);

        return path;
    }

    private static string InnerGetTemporaryDirectoryPath(Assembly assembly) {
        var root = InnerGetRootDirectoryPath(assembly);
        var path = Path.Combine(root, TEMPORARY_DIRECTORY_NAME);

        // Ensure directory existence.
        Directory.CreateDirectory(path);

        return path;
    }
}