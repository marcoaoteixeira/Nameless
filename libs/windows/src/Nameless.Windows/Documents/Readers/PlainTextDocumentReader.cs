using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.ObjectModel;

namespace Nameless.Windows.Documents.Readers;

/// <summary>
///     Plain-text Document Reader
/// </summary>
public class PlainTextDocumentReader : IDocumentReader {
    private readonly ILogger<PlainTextDocumentReader> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PlainTextDocumentReader"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public PlainTextDocumentReader(ILogger<PlainTextDocumentReader> logger) {
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool CanRead(string path) {
        return string.Equals(
            SysPath.GetExtension(path),
            WellKnownDocumentExtensions.Txt,
            StringComparison.OrdinalIgnoreCase
        );
    }

    /// <inheritdoc />
    public async Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(path);

        try {
            if (SysFile.Exists(path)) {
                return await SysFile.ReadAllTextAsync(path, cancellationToken)
                                    .SkipContextSync();
            }

            return string.Empty;
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }
    }
}