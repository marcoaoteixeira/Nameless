using Xunit;

namespace Nameless.Testing.Tools.Data;

public class StringNullEmptyWhiteSpaceTheoryData : TheoryData<string?, Type> {
    public StringNullEmptyWhiteSpaceTheoryData() {
        Add(null, typeof(ArgumentNullException));
        Add(string.Empty, typeof(ArgumentException));
        Add(" ", typeof(ArgumentException));
    }
}