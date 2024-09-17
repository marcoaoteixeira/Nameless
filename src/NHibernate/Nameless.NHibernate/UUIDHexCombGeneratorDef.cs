using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate;

/// <summary>
/// UUID hex comb implementation for <see cref="IGeneratorDef"/>.
/// It uses singleton pattern. See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class UUIDHexCombGeneratorDef : IGeneratorDef {
    /// <summary>
    /// Gets the unique instance of <see cref="UUIDHexCombGeneratorDef" />.
    /// </summary>
    public static IGeneratorDef Instance { get; } = new UUIDHexCombGeneratorDef();

    /// <inheritdoc />
    public string Class => "uuid.hex";

    /// <inheritdoc />
    public object Params => new { format = "D" };

    /// <inheritdoc />
    public Type DefaultReturnType => typeof(string);

    /// <inheritdoc />
    public bool SupportedAsCollectionElementId => false;

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static UUIDHexCombGeneratorDef() { }

    private UUIDHexCombGeneratorDef() { }
}