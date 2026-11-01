using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.IO;
using Nameless.ObjectModel;
using Nameless.Windows.Office;
using System.Security.Cryptography;
using System.Text;

namespace Nameless.Windows.Documents.Converters;

/// <summary>
///     XPS Document Converter
/// </summary>
public class XpsDocumentConverter : IDocumentConverter {
    private static readonly DocumentType[] Accepts = [
        DocumentType.Docx,
        DocumentType.Doc,
        DocumentType.Rtf,
        DocumentType.Txt
    ];

    private readonly IFileManager _fileManager;
    private readonly IWordApplication _wordApplication;
    private readonly ILogger<XpsDocumentConverter> _logger;

    /// <inheritdoc />
    public DocumentType OutputType => DocumentType.Xps;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="XpsDocumentConverter"/> class.
    /// </summary>
    /// <param name="fileManager">The file manager.</param>
    /// <param name="wordApplication">The Word application.</param>
    /// <param name="logger">The logger</param>
    public XpsDocumentConverter(IFileManager fileManager, IWordApplication wordApplication, ILogger<XpsDocumentConverter> logger) {
        _fileManager = Throws.When.Null(fileManager);
        _wordApplication = Throws.When.Null(wordApplication);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool CanConvert(DocumentType from) {
        return Accepts.Contains(from);
    }

    /// <inheritdoc />
    public Task<ConversionResult> ConvertAsync(string path, CancellationToken cancellationToken) {
        ConversionResult result;

        var tempDir = _fileManager.GetTemporaryDirectory();

        tempDir.Create(); // Ensure existence

        var tempFileName = GetTemporaryFileName(path);

        var relativeFilePath = SysPath.Combine(
            SysPath.GetRelativePath(_fileManager.Root, tempDir.Path),
            string.Concat(tempFileName, ".xps")
        );

        var file = _fileManager.GetFile(relativeFilePath);

        try {
            using var document = _wordApplication.Open(path);

            document.SaveAs(file.Path, DocumentFormat.Xps);

            result = file.Path;
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            result = Error.Failure(ex.Message, ex: ex);
        }

        return Task.FromResult(result);
    }

    private static string GetTemporaryFileName(string filePath) {
        filePath = PathHelper.Normalize(filePath);

        Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(filePath)];
        Encoding.UTF8.GetBytes(filePath, buffer);

        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(buffer, hash);

        return Convert.ToHexStringLower(hash);
    }
}