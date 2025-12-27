using Xunit;

namespace Nameless.Testing.Tools.Data;

public class StringNullEmptyWhiteSpaceExceptionInlineData : TheoryData<string?, Type> {
    public StringNullEmptyWhiteSpaceExceptionInlineData() {
        Add(p1: null, typeof(ArgumentNullException));
        Add(string.Empty, typeof(ArgumentException));
        Add(p1: " ", typeof(ArgumentException));
    }
}