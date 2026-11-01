namespace Nameless.Windows.Documents.Readers;

/// <summary>
///     Document Reader Contract
/// </summary>
public interface IDocumentReader {
    /// <summary>
    ///     Checks if the current reader can read the specified document.
    /// </summary>
    /// <param name="path">
    ///     The document file path.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if it can; otherwise, <see langword="false"/>.
    /// </returns>
    bool CanRead(string path);

    /// <summary>
    ///     Retrieves the document content.
    /// </summary>
    /// <param name="path">
    ///     The document file path.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous method
    ///     execution.
    /// </returns>
    Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken);
}