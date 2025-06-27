using Xunit;

namespace Nameless.Testing.Tools.Data;
public sealed class ArrayNullEmptyExceptionInlineData<TArray> : TheoryData<TArray[]?, Type> {

    public ArrayNullEmptyExceptionInlineData() {
        Add(null, typeof(ArgumentNullException));
        Add([], typeof(ArgumentException));
    }
}
