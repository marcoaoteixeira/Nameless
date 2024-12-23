namespace Nameless.FileSystem;

/// <summary>
/// File service contract.
/// </summary>
public interface IFileService {
    /// <summary>
    /// Copies the file at <paramref name="sourceFilePath"/> to a new file at <paramref name="destinationFilePath"/>.
    /// </summary>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <param name="destinationFilePath">The destination file path.</param>
    /// <param name="overwrite">Whether it will overwrite the existent file.</param>
    void Copy(string sourceFilePath, string destinationFilePath, bool overwrite);

    /// <summary>
    /// Opens a stream to a file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="mode">The file mode flag.</param>
    /// <param name="access">The file access flag.</param>
    /// <param name="share">The file share flag.</param>
    /// <returns>A <see cref="Stream"/> representing the opened file.</returns>
    Stream Open(string filePath, FileMode mode, FileAccess access, FileShare share);

    /// <summary>
    /// Creates a new file at the <paramref name="filePath"/> and returns its <see cref="Stream"/>.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A <see cref="Stream"/> representing the newly created file.</returns>
    Stream Create(string filePath);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    void Delete(string filePath);

    /// <summary>
    /// Checks if the specified file exists.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    bool Exists(string filePath);

    /// <summary>
    /// Reads the specified file to a <see cref="string"/>.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A <see cref="string"/> representing the file content.</returns>
    string ReadAllText(string filePath);

    /// <summary>
    /// Reads all file content to a <see cref="byte"/> array.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A <see cref="byte"/> array representing the file.</returns>
    byte[] ReadAllBytes(string filePath);
}

public static class FileOperatorExtension {
    /// <summary>
    /// Copies the file at <paramref name="sourceFilePath"/> to a new file at <paramref name="destinationFilePath"/>.
    /// </summary>
    /// <param name="self">The current instance of <see cref="IFileService"/>.</param>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <param name="destinationFilePath">The destination file path.</param>
    public static void Copy(this IFileService self,
                            string sourceFilePath,
                            string destinationFilePath)
        => self.Copy(sourceFilePath, destinationFilePath, overwrite: false);

    /// <summary>
    /// Opens a stream to a file.
    /// </summary>
    /// <param name="self">The current instance of <see cref="IFileService"/>.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="mode">The file mode flag. Default is <see cref="FileMode.Open"/>.</param>
    /// <param name="access">The file access flag. Default is <see cref="FileAccess.Read"/>.</param>
    /// <param name="share">The file share flag. Default is <see cref="FileShare.ReadWrite"/>.</param>
    /// <returns>A <see cref="Stream"/> representing the opened file.</returns>
    public static Stream Open(this IFileService self,
                              string filePath,
                              FileMode mode = FileMode.Open,
                              FileAccess access = FileAccess.Read,
                              FileShare share = FileShare.ReadWrite)
        => self.Open(filePath, mode, access, share);
}