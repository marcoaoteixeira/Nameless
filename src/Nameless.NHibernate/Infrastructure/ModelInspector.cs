using Nameless;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Infrastructure;

/// <summary>
///     Default implementation of <see cref="ExplicitlyDeclaredModel" />.
/// </summary>
public class ModelInspector : ExplicitlyDeclaredModel {
    private readonly Type[] _entityTypes;

    /// <summary>
    ///     Initializes a new instance of <see cref="ModelInspector" />
    /// </summary>
    public ModelInspector(Type[] entityTypes) {
        _entityTypes = Guard.Against.Null(entityTypes);
    }

    /// <inheritdoc />
    public override bool IsEntity(Type type) {
        return _entityTypes.Any(entityType =>
            entityType.IsGenericType
                ? entityType.IsAssignableFromGenericType(type)
                : entityType.IsAssignableFrom(type));
    }
}