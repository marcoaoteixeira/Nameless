using Nameless.Registration;

namespace Nameless.WPF.Documents;

public class DocumentServicesRegistration : AssemblyScanAware<DocumentServicesRegistration> {
    private readonly HashSet<Type> _documentConverters = [];
    private readonly HashSet<Type> _documentReaders = [];

    public IReadOnlyCollection<Type> DocumentConverters => _documentConverters;

    public IReadOnlyCollection<Type> DocumentReaders => _documentReaders;

    public DocumentServicesRegistration RegisterDocumentConverter<TDocumentConverter>()
        where TDocumentConverter : IDocumentConverter {
        _documentConverters.Add(typeof(TDocumentConverter));

        return this;
    }

    public DocumentServicesRegistration RegisterDocumentReader<TDocumentReader>()
        where TDocumentReader : IDocumentReader {
        _documentReaders.Add(typeof(TDocumentReader));

        return this;
    }
}