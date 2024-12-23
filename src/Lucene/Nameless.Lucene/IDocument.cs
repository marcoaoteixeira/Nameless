using Lucene.Net.Index;

namespace Nameless.Lucene;

public interface IDocument {
    IIndexableField? GetField(string name);
}