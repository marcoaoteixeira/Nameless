using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.ObjectModel;
using Nameless.Windows.Office;

namespace Nameless.Windows.Documents.Readers;

/// <summary>
///     Microsoft Word Document Reader
/// </summary>
public class WordDocumentReader : IDocumentReader {
    private readonly IWordApplication _wordApplication;
    private readonly ILogger<WordDocumentReader> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="WordDocumentReader"/> class.
    /// </summary>
    /// <param name="wordApplication">
    ///     The Word application.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public WordDocumentReader(IWordApplication wordApplication, ILogger<WordDocumentReader> logger) {
        _wordApplication = Throws.When.Null(wordApplication);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool CanRead(string path) {
        var extension = SysPath.GetExtension(path);

        return string.Equals(extension, WellKnownDocumentExtensions.Doc, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, WellKnownDocumentExtensions.Docx, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, WellKnownDocumentExtensions.Rtf, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken) {
        ContentResult result;

        try {
            using var document = _wordApplication.Open(path);

            result = document.GetContent(formatted: false);
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            result = Error.Failure(ex.Message, ex: ex);
        }

        return Task.FromResult(result);
    }
}