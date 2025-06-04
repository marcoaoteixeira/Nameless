using System.Collections;

namespace Nameless.Search.Lucene.Fixtures;

public sealed class FieldOptionsInlineData : IEnumerable<object[]> {
    public IEnumerator<object[]> GetEnumerator() {
        foreach (var value in Enum.GetValues<FieldOptions>()) {
            yield return [value];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
