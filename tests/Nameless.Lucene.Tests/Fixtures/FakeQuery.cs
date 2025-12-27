using Lucene.Net.Search;

namespace Nameless.Lucene.Fixtures;

public class FakeQuery : Query {
    public override string ToString(string field) {
        return field;
    }
}
