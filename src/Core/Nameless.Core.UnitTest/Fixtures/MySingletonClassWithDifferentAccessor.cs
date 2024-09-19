namespace Nameless.Fixtures;

[Singleton(AccessorName = "Default")]
public sealed class MySingletonClassWithDifferentAccessor {

    public static MySingletonClassWithDifferentAccessor Default { get; } = new();

    private MySingletonClassWithDifferentAccessor() { }

    static MySingletonClassWithDifferentAccessor() { }
}