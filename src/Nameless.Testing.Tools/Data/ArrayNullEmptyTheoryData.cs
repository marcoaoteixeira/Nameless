using Xunit;

namespace Nameless.Testing.Tools.Data;

public class ArrayNullEmptyTheoryData<TArray> : TheoryData<TArray[]?, Type> {
    public ArrayNullEmptyTheoryData() {
        Add(null, typeof(ArgumentNullException));
        Add([], typeof(ArgumentException));
    }
}