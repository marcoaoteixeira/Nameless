using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate;

/// <summary>
/// Default implementation of <see cref="ExplicitlyDeclaredModel" />.
/// </summary>
public sealed class ModelInspector : ExplicitlyDeclaredModel {
    private readonly Type[] _entityTypes;

    /// <summary>
    /// Initializes a new instance of <see cref="ModelInspector" />
    /// </summary>
    public ModelInspector(Type[] entityTypes) {
        _entityTypes = Prevent.Argument.Null(entityTypes);
    }

    /// <inheritdoc />
    public override bool IsEntity(Type type)
        => _entityTypes.Any(entityType =>
                                entityType.IsGenericType
                                    ? entityType.IsAssignableFromOpenGenericType(type)
                                    : entityType.IsAssignableFrom(type));
}