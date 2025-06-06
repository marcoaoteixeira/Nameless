using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate;

/// <summary>
///     UUID hex comb implementation for <see cref="IGeneratorDef" />.
/// </summary>
public sealed class UuidHexCombGeneratorDef : IGeneratorDef {
    /// <inheritdoc />
    public string Class => "uuid.hex";

    /// <inheritdoc />
    public object Params => new { format = "D" };

    /// <inheritdoc />
    public Type DefaultReturnType => typeof(string);

    /// <inheritdoc />
    public bool SupportedAsCollectionElementId => false;
}