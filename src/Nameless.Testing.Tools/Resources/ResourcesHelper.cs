using System.Reflection;
using Nameless.Helpers;

namespace Nameless.Testing.Tools.Resources;

public static class ResourcesHelper {
    private const string ROOT_DIRECTORY_NAME = "Resources";

    public static Resource GetResource(string relativePath, bool createCopy = true) {
        var resourceFilePath = GetResourceFilePath(Assembly.GetCallingAssembly(), relativePath);

        return new Resource(
            path: createCopy ? CreateResourceCopy(resourceFilePath) : resourceFilePath,
            deleteOnDispose: createCopy
        );
    }

    public static Resource CreateTemporary(string extension = ".dat") {
        var resourceFilePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.CreateVersion7():N}{extension}"
        );

        return new Resource(resourceFilePath, deleteOnDispose: true);
    }

    private static string GetResourceFilePath(Assembly assembly, string relativeFilePath) {
        var root = Path.Combine(assembly.GetDirectoryPath(), ROOT_DIRECTORY_NAME);

        Directory.CreateDirectory(root);

        var filePath = Path.IsPathRooted(relativeFilePath)
            ? Path.GetFullPath(relativeFilePath)
            : Path.GetFullPath(relativeFilePath, root);

        filePath = PathHelper.Normalize(filePath);

        if (!filePath.StartsWith(root)) {
            throw new UnauthorizedAccessException("The specified path is outside the root directory.");
        }

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("Couldn't locate the specified file.", relativeFilePath);
        }

        return filePath;
    }

    private static string CreateResourceCopy(string sourceFilePath) {
        var fileExtension = Path.GetExtension(sourceFilePath);
        var fileName = $"{Guid.CreateVersion7():N}{fileExtension}";

        var destinationFilePath = Path.Combine(Path.GetTempPath(), fileName);

        Directory.CreateDirectory(Path.GetTempPath());
        File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

        return destinationFilePath;
    }
}