using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.ObjectModel;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Nameless.Windows.Documents.Readers;

/// <summary>
///     PDF Document Reader
/// </summary>
public class PdfDocumentReader : IDocumentReader {
    private readonly ILogger<PdfDocumentReader> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PdfDocumentReader"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public PdfDocumentReader(ILogger<PdfDocumentReader> logger) {
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool CanRead(string path) {
        return string.Equals(
            SysPath.GetExtension(path),
            WellKnownDocumentExtensions.Pdf,
            StringComparison.OrdinalIgnoreCase
        );
    }

    /// <inheritdoc />
    public Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken) {
        var sb = new StringBuilder();

        ContentResult result;

        try {
            using var document = PdfDocument.Open(path);

            foreach (var page in document.GetPages()) {
                var content = ContentOrderTextExtractor.GetText(page);

                sb.Append(content);
            }

            result = sb.ToString();
        }
        catch (Exception ex) {
            result = Error.Failure(ex.Message, ex: ex);

            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);
        }

        return Task.FromResult(result);
    }
}