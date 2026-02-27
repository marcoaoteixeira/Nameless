using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Mapping;
using Nameless.Lucene.Requests;

namespace Nameless.Lucene.Collections;

public class StreamAsyncEnumerable<TDocument> : IAsyncEnumerable<TDocument>
    where TDocument : class, new() {
    private readonly EntityDocumentBuilder _entityDocumentBuilder;
    private readonly IIndex _index;
    private readonly SearchDocumentsRequest _request;

    public StreamAsyncEnumerable(EntityDocumentBuilder entityDocumentBuilder, IIndex index, SearchDocumentsRequest request) {
        _entityDocumentBuilder = entityDocumentBuilder;
        _index = index;
        _request = request;
    }

    public IAsyncEnumerator<TDocument> GetAsyncEnumerator(CancellationToken cancellationToken) {
        return new StreamAsyncEnumerator<TDocument>(_entityDocumentBuilder, _index, _request, cancellationToken);
    }
}
