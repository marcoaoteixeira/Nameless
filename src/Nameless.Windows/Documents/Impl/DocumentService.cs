using System.IO;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.Windows.Localization;

namespace Nameless.Windows.Documents.Impl;

public class DocumentService : IDocumentService {
    private const string CLASS = nameof(DocumentService);

    private readonly IEnumerable<IDocumentReader> _documentReaders;
    private readonly IEnumerable<IDocumentConverter> _documentConverters;
    
    private ILocalizer T { get; }

    public DocumentService(IEnumerable<IDocumentReader> documentReaders, IEnumerable<IDocumentConverter> documentConverters, ILocalizer localizer) {
        _documentReaders = documentReaders;
        _documentConverters = documentConverters;

        T = localizer;
    }

    public async Task<string> GetContentAsync(string filePath, CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(filePath);

        var reader = _documentReaders.FirstOrDefault(
            reader => reader.CanRead(filePath)
        );

        if (reader is not null) {
            return await reader.GetContentAsync(filePath, cancellationToken)
                               .ConfigureAwait(continueOnCapturedContext: false);
        }

        return string.Empty;
    }

    public async Task<Result<string>> ConvertAsync(string filePath, DocumentType output, CancellationToken cancellationToken) {
        const string ActionName = nameof(ConvertAsync);

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var fileType = extension switch {
            ".doc" or ".docx" => DocumentType.DOCX,
            ".rtf" => DocumentType.RTF,
            ".xps" => DocumentType.XPS,
            ".txt" => DocumentType.TXT,
            ".pdf" => DocumentType.PDF,
            _ => DocumentType.None
        };

        if (fileType == DocumentType.None) {
            return Error.Conflict(
                T[$"{CLASS}_{ActionName}_UnknownDocumentExtension", extension]
            );
        }

        // Find a converter that can handle the specified document type
        var converter = _documentConverters.FirstOrDefault(
            converter => converter.CanConvert(fileType) && converter.OutputType == output
        );

        if (converter is null) {
            return Error.Conflict(
                T[$"{CLASS}_{ActionName}_MissingDocumentConverter", fileType]
            );
        }

        return await converter.ConvertAsync(filePath, cancellationToken);
    }
}