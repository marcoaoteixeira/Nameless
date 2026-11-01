namespace Nameless.Windows.Office;

/// <summary>
///     Microsoft Word Document Contract
/// </summary>
public interface IWordDocument : IDisposable {
    /// <summary>
    ///     Gets the document status.
    /// </summary>
    WordDocumentStatus Status { get; }

    /// <summary>
    ///     Retrieves the document content.
    /// </summary>
    /// <param name="formatted">
    ///     Whether it should also retrieve the formatting marks.
    /// </param>
    /// <returns></returns>
    string GetContent(bool formatted);

    /// <summary>
    ///     Saves the current document as the specified format.
    /// </summary>
    /// <param name="filePath">
    ///     Where to save the document.
    /// </param>
    /// <param name="format">
    ///     Format to save.
    /// </param>
    void SaveAs(string filePath, DocumentFormat format);
}