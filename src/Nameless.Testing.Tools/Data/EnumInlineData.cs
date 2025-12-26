using Xunit;

namespace Nameless.Testing.Tools.Data;

public sealed class EnumInlineData<TEnum> : TheoryData<TEnum>
    where TEnum : struct, Enum {
    public EnumInlineData() {
        foreach (var @enum in Enum.GetValues<TEnum>()) {
            Add(@enum);
        }
    }
}