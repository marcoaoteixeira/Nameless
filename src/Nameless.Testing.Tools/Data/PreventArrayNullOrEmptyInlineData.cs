using System.Collections;
using Xunit;

namespace Nameless.Testing.Tools.Data;
public class PreventArrayNullOrEmptyInlineData<TArray> : IEnumerable<TheoryDataRow<TArray[]?, Type>> {
    public IEnumerator<TheoryDataRow<TArray[]?, Type>> GetEnumerator() {
        yield return new TheoryDataRow<TArray[]?, Type>(null, typeof(ArgumentNullException));
        yield return new TheoryDataRow<TArray[]?, Type>([], typeof(ArgumentException));
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
