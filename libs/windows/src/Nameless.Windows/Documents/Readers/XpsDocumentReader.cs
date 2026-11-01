using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace Nameless.Windows.Documents.Readers;

/// <summary>
///     XPS Document Reader
/// </summary>
public class XpsDocumentReader : IDocumentReader {
    private readonly ILogger<XpsDocumentReader> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="XpsDocumentReader"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public XpsDocumentReader(ILogger<XpsDocumentReader> logger) {
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool CanRead(string path) {
        return string.Equals(
            SysPath.GetExtension(path),
            WellKnownDocumentExtensions.Xps,
            StringComparison.OrdinalIgnoreCase
        );
    }

    /// <inheritdoc />
    public Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken) {
        var tcs = new TaskCompletionSource<ContentResult>();

        var thread = new Thread(() => {
            try { tcs.SetResult(GetContent(path)); }
            catch (Exception ex) { tcs.SetException(ex); }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return tcs.Task;
    }

    private ContentResult GetContent(string filePath) {
        var sb = new StringBuilder();

        try {
            using var xpsDocument = new XpsDocument(filePath, FileAccess.Read);

            var fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();
            if (fixedDocumentSequence is null) {

                return string.Empty;
            }

            foreach (var reference in fixedDocumentSequence.References) {
                var document = reference.GetDocument(forceReload: false);
                if (document is null) {
                    continue;
                }

                foreach (var pageContent in document.Pages) {
                    var page = pageContent.GetPageRoot(forceReload: false);

                    // Recursively extract text from visual tree
                    ExtractElementContent(page, sb);
                }
            }
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }

        return sb.ToString();
    }

    private static void ExtractElementContent(DependencyObject? element, StringBuilder builder) {
        switch (element) {
            case null:
                return;
            // If the element is a TextBlock, append its text
            case TextBlock textBlock:
                builder.AppendLine(textBlock.Text);
                break;
            // If the element is a Glyphs (common in XPS), extract its UnicodeString
            case Glyphs glyphs when !string.IsNullOrEmpty(glyphs.UnicodeString):
                builder.Append(glyphs.UnicodeString);
                break;
        }

        // Recursively search children
        var count = VisualTreeHelper.GetChildrenCount(element);
        for (var index = 0; index < count; index++) {
            ExtractElementContent(VisualTreeHelper.GetChild(element, index), builder);
        }
    }
}