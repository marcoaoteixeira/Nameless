namespace Nameless.Windows.Documents.Converters;

/// <summary>
///     Document Converter Contract
/// </summary>
public interface IDocumentConverter {
    /// <summary>
    ///     Gets the converter output document type.
    /// </summary>
    DocumentType OutputType { get; }

    /// <summary>
    ///     Checks if the current converter can convert the specified document.
    /// </summary>
    /// <param name="from">
    ///     From document type.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if it can; otherwise, <see langword="false"/>.
    /// </returns>
    bool CanConvert(DocumentType from);

    /// <summary>
    ///     Converts the document.
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
    Task<ConversionResult> ConvertAsync(string path, CancellationToken cancellationToken);
}