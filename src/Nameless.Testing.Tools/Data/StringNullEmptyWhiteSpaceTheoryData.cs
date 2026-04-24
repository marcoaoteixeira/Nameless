using Xunit;

namespace Nameless.Testing.Tools.Data;

/// <summary>
///     Generates test data for string values <c>null</c>, empty and white space.
///     Also provides the expected exception.
/// </summary>
public class StringNullEmptyWhiteSpaceTheoryData : TheoryData<string?, Type> {
    /// <summary>
    ///     Initializes a new instance of <see cref="StringNullEmptyWhiteSpaceTheoryData"/> class.
    /// </summary>
    public StringNullEmptyWhiteSpaceTheoryData() {
        Add(null, typeof(ArgumentNullException));
        Add(string.Empty, typeof(ArgumentException));
        Add(" ", typeof(ArgumentException));
    }
}