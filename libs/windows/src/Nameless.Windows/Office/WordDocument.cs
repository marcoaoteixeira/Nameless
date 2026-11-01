using System.Text;
using Nameless.Contracts;

namespace Nameless.Windows.Office;

/// <summary>
///     Microsoft Word Document
/// </summary>
public sealed class WordDocument : IWordDocument {
    private MSWord_Document? _document;
    private object _missing = Type.Missing;
    private bool _disposed;

    /// <inheritdoc/>
    public WordDocumentStatus Status { get; private set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="WordDocument"/> class.
    /// </summary>
    /// <param name="document">
    ///     The actual document.
    /// </param>
    public WordDocument(MSWord_Document document) {
        _document = Throws.When.Null(document);

        Initialize();
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~WordDocument() {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public string GetContent(bool formatted) {
        BlockAccessAfterDispose();

        var sb = new StringBuilder();

        // Read header
        foreach (MSWord_Section section in GetDocument().Sections) {
            foreach (MSWord_HeaderFooter header in section.Headers) {
                sb.AppendLine(ExtractText(header.Range, formatted));
            }
        }

        // Read content
        sb.AppendLine(ExtractText(GetDocument().Content, formatted));

        // Read footer
        foreach (MSWord_Section section in GetDocument().Sections) {
            foreach (MSWord_HeaderFooter footer in section.Footers) {
                sb.AppendLine(ExtractText(footer.Range, formatted));
            }
        }

        return sb.ToString();

        static string ExtractText(MSWord_Range range, bool formatted) {
            return formatted ? range.FormattedText.Text : range.Text;
        }
    }

    /// <inheritdoc/>
    public void SaveAs(string filePath, DocumentFormat format) {
        BlockAccessAfterDispose();

        var currentFormat = (object)Convert(format);
        object? currentOutputFilePath = filePath;

        GetDocument().SaveAs2(
            FileName: ref currentOutputFilePath,
            FileFormat: ref currentFormat,
            LockComments: ref _missing,
            Password: ref _missing,
            AddToRecentFiles: ref _missing,
            WritePassword: ref _missing,
            ReadOnlyRecommended: ref _missing,
            EmbedTrueTypeFonts: ref _missing,
            SaveNativePictureFormat: ref _missing,
            SaveFormsData: ref _missing,
            SaveAsAOCELetter: ref _missing,
            Encoding: ref _missing,
            InsertLineBreaks: ref _missing,
            AllowSubstitutions: ref _missing,
            LineEnding: ref _missing,
            AddBiDiMarks: ref _missing,
            CompatibilityMode: ref _missing
        );
    }

    /// <inheritdoc/>
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _document?.Close(
                SaveChanges: ref _missing,
                OriginalFormat: ref _missing,
                RouteDocument: ref _missing
            );
        }

        _document = null;
        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Initialize() {
        if (_document is not MSWord_DocumentEvents_Event evt) {
            return;
        }

        evt.Close += OnClose;
        Status = WordDocumentStatus.Opened;
    }

    private void OnClose() {
        if (_document is not MSWord_DocumentEvents_Event evt) {
            return;
        }

        evt.Close -= OnClose;
        Status = WordDocumentStatus.Closed;
    }

    private MSWord_Document GetDocument() {
        return _document ?? throw new ArgumentNullException(nameof(_document));
    }

    private static MSWord_WdSaveFormat Convert(DocumentFormat type) {
        return type switch {
            DocumentFormat.Docx => MSWord_WdSaveFormat.wdFormatXMLDocument,
            DocumentFormat.Doc => MSWord_WdSaveFormat.wdFormatDocument,
            DocumentFormat.Rtf => MSWord_WdSaveFormat.wdFormatRTF,
            DocumentFormat.Xps => MSWord_WdSaveFormat.wdFormatXPS,
            _ => throw new InvalidOperationException("Invalid document format.")
        };
    }
}