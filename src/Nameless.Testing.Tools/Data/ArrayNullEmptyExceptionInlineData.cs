using Xunit;

namespace Nameless.Testing.Tools.Data;

public sealed class ArrayNullEmptyExceptionInlineData<TArray> : TheoryData<TArray[]?, Type> {
    public ArrayNullEmptyExceptionInlineData() {
        Add(p1: null, typeof(ArgumentNullException));
        Add([], typeof(ArgumentException));
    }
}