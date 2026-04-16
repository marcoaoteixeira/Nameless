using Xunit;

namespace Nameless.Testing.Tools.Data;

public sealed class EnumTheoryData<TEnum> : TheoryData<TEnum>
    where TEnum : struct, Enum {
    public EnumTheoryData() {
        foreach (var @enum in Enum.GetValues<TEnum>()) {
            Add(@enum);
        }
    }
}