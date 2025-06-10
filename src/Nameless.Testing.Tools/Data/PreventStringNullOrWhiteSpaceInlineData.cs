using System.Collections;
using Xunit;

namespace Nameless.Testing.Tools.Data;

public class PreventStringNullOrWhiteSpaceInlineData : IEnumerable<TheoryDataRow<string?, Type>> {
    public IEnumerator<TheoryDataRow<string?, Type>> GetEnumerator() {
        yield return new TheoryDataRow<string?, Type>(null, typeof(ArgumentNullException));
        yield return new TheoryDataRow<string?, Type>(string.Empty, typeof(ArgumentException));
        yield return new TheoryDataRow<string?, Type>(" ", typeof(ArgumentException));
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
