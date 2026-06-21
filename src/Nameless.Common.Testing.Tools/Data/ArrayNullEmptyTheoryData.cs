using Xunit;

namespace Nameless.Testing.Tools.Data;

/// <summary>
///     Provides test data where value is an array of <typeparamref name="TArray"/>
///     and assert a particular exception.
/// </summary>
/// <typeparam name="TArray">
///     Type of the array.
/// </typeparam>
public class ArrayNullEmptyTheoryData<TArray> : TheoryData<TArray[]?, Type> {
    /// <summary>
    ///     Initializes a new instance of <see cref="ArrayNullEmptyTheoryData{TArray}"/> class.
    /// </summary>
    public ArrayNullEmptyTheoryData() {
        Add(null, typeof(ArgumentNullException));
        Add([], typeof(ArgumentException));
    }
}