using Lucene.Net.Index;

namespace Nameless.Search.Lucene.Null;

public sealed class NullFields : Fields {
    public static Fields Instance { get; } = new NullFields();

    static NullFields() { }

    private NullFields() { }

    public override int Count => 0;

    public override IEnumerator<string> GetEnumerator() {
        return Enumerable.Empty<string>().GetEnumerator();
    }

    public override Terms GetTerms(string field) {
        return NullTerms.Instance;
    }
}