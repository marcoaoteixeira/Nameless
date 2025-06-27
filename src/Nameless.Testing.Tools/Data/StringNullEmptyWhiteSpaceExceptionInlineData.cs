using Xunit;

namespace Nameless.Testing.Tools.Data;

public class StringNullEmptyWhiteSpaceExceptionInlineData : TheoryData<string?, Type> {
    public StringNullEmptyWhiteSpaceExceptionInlineData() {
        Add(null, typeof(ArgumentNullException));
        Add(string.Empty, typeof(ArgumentException));
        Add(" ", typeof(ArgumentException));
    }
}
