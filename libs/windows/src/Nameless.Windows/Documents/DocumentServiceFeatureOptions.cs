using Nameless.Contracts;
using Nameless.Registration;
using Nameless.Windows.Documents.Converters;
using Nameless.Windows.Documents.Readers;

namespace Nameless.Windows.Documents;

/// <summary>
///     Document Feature Options
/// </summary>
public class DocumentServiceFeatureOptions : AssemblyScanAware<DocumentServiceFeatureOptions> {
    private static readonly Type DocumentConverterType = typeof(IDocumentConverter);
    private static readonly Type DocumentReaderType = typeof(IDocumentReader);

    private readonly HashSet<Type> _documentConverters = [];
    private readonly HashSet<Type> _documentReaders = [];

    /// <summary>
    ///     Gets the document converter implementations.
    /// </summary>
    public IReadOnlyCollection<Type> DocumentConverters => UseAssemblyScan
        ? ExecuteAssemblyScan(DocumentConverterType)
        : _documentConverters;

    /// <summary>
    ///     Gets the document reader implementations.
    /// </summary>
    public IReadOnlyCollection<Type> DocumentReaders => UseAssemblyScan
        ? ExecuteAssemblyScan(DocumentReaderType)
        : _documentReaders;

    /// <summary>
    ///     Registers the document converter.
    /// </summary>
    /// <typeparam name="TDocumentConverter">
    ///     Type of the document converter.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="DocumentServiceFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public DocumentServiceFeatureOptions WithDocumentConverter<TDocumentConverter>() where TDocumentConverter : IDocumentConverter {
        return WithDocumentConverter(typeof(TDocumentConverter));
    }

    /// <summary>
    ///     Registers the document converter.
    /// </summary>
    /// <param name="type">
    ///     The document converter type.
    /// </param>
    /// <returns>
    ///     The current <see cref="DocumentServiceFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public DocumentServiceFeatureOptions WithDocumentConverter(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, lhs: DocumentConverterType);

        _documentConverters.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers the document reader.
    /// </summary>
    /// <typeparam name="TDocumentReader">
    ///     Type of the document reader.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="DocumentServiceFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public DocumentServiceFeatureOptions WithDocumentReader<TDocumentReader>() where TDocumentReader : IDocumentReader {
        return WithDocumentReader(typeof(TDocumentReader));
    }

    /// <summary>
    ///     Registers the document reader.
    /// </summary>
    /// <param name="type">
    ///     Type of the document reader.
    /// </param>
    /// <returns>
    ///     The current <see cref="DocumentServiceFeatureOptions"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public DocumentServiceFeatureOptions WithDocumentReader(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, lhs: DocumentReaderType);

        _documentReaders.Add(type);

        return this;
    }
}