namespace Nameless.Fixtures;

public sealed class MySingletonClassWithoutAttribute {
    public static MySingletonClassWithoutAttribute Instance { get; } = new();

    private MySingletonClassWithoutAttribute() { }

    static MySingletonClassWithoutAttribute() { }
}