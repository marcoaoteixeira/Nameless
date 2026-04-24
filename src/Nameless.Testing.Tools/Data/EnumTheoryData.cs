using Xunit;

namespace Nameless.Testing.Tools.Data;

/// <summary>
///     Generate test data for enums.
/// </summary>
/// <typeparam name="TEnum">
///     Type of the enum.
/// </typeparam>
public sealed class EnumTheoryData<TEnum> : TheoryData<TEnum>
    where TEnum : struct, Enum {
    /// <summary>
    ///     Initializes a new instance of <see cref="EnumTheoryData{TEnum}"/> class.
    /// </summary>
    public EnumTheoryData() {
        foreach (var @enum in Enum.GetValues<TEnum>()) {
            Add(@enum);
        }
    }
}