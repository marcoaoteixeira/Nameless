using Nameless.ObjectModel;

namespace Nameless.Windows.Documents;

/// <summary>
///     Document Service Contract
/// </summary>
public interface IDocumentService {
    /// <summary>
    ///     Retrieves the content of a document.
    /// </summary>
    /// <param name="path">
    ///     The file path to the document.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous action
    ///     execution. The result of the action is a <see cref="string"/>
    ///     representing the document content.
    /// </returns>
    Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Converts a document output the specified type.
    /// </summary>
    /// <param name="path">
    ///     The document file path.
    /// </param>
    /// <param name="output">
    ///     Document type output.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous action
    ///     where the result is a <see cref="Result{T}"/> containing
    ///     the path output the converted document.
    /// </returns>
    Task<ConversionResult> ConvertAsync(string path, DocumentType output, CancellationToken cancellationToken);
}