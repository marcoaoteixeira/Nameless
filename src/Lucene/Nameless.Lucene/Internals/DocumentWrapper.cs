using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Nameless.Lucene.Internals;

internal sealed class DocumentWrapper : IDocument {
    private readonly Document _document;

    internal DocumentWrapper(Document document) {
        _document = Prevent.Argument.Null(document);
    }

    IIndexableField? IDocument.GetField(string name)
        => _document.GetField(name);
}