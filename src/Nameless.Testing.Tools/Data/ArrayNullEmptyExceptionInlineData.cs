using Xunit;

namespace Nameless.Testing.Tools.Data;

public class ArrayNullEmptyExceptionInlineData<TArray> : TheoryData<TArray[]?, Type> {
    public ArrayNullEmptyExceptionInlineData() {
        Add(null, typeof(ArgumentNullException));
        Add([], typeof(ArgumentException));
    }
}