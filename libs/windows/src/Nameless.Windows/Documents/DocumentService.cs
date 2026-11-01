using Microsoft.Extensions.DependencyInjection;
using Nameless.Contracts;
using Nameless.ObjectModel;
using Nameless.Windows.Documents.Converters;
using Nameless.Windows.Documents.Readers;

namespace Nameless.Windows.Documents;

/// <summary>
///     Document Service
/// </summary>
public class DocumentService : IDocumentService {
    private readonly IDocumentReader[] _documentReaders;
    private readonly IDocumentConverter[] _documentConverters;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="DocumentService"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public DocumentService(IServiceProvider provider) {
        Throws.When.Null(provider);

        _documentReaders = [.. provider.GetServices<IDocumentReader>()];
        _documentConverters = [.. provider.GetServices<IDocumentConverter>()];
    }

    /// <inheritdoc />
    public async Task<ContentResult> GetContentAsync(string path, CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(path);

        var reader = _documentReaders.FirstOrDefault(
            reader => reader.CanRead(path)
        );

        if (reader is not null) {
            return await reader.GetContentAsync(path, cancellationToken)
                               .SkipContextSync();
        }

        return Error.Missing("No reader available.");
    }

    /// <inheritdoc />
    public async Task<ConversionResult> ConvertAsync(string path, DocumentType output, CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(path);

        var originExtension = SysPath.GetExtension(path)[1..];

        if (!Enum.TryParse<DocumentType>(originExtension, ignoreCase: true, out var from)) {
            return Error.Conflict($"Unknown document type '{originExtension}'.");
        }

        var converter = _documentConverters.FirstOrDefault(
            converter => converter.OutputType == output && converter.CanConvert(from)
        );

        if (converter is null) {
            return Error.Conflict($"No document converter available output convert from '{from}' to '{output}'.");
        }

        return await converter.ConvertAsync(path, cancellationToken);
    }
}