using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Impl;

/// <summary>
/// Default implementation of <see cref="ExplicitlyDeclaredModel" />.
/// </summary>
public sealed class ModelInspector : ExplicitlyDeclaredModel {
    #region Private Read-Only Fields

    private readonly Type[] _entityTypes;

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ModelInspector" />
    /// </summary>
    public ModelInspector(Type[] entityTypes) {
        _entityTypes = Prevent.Argument.Null(entityTypes, nameof(entityTypes));
    }

    #endregion

    #region Public Override Methods

    /// <inheritdoc />
    public override bool IsEntity(Type type)
        => _entityTypes.Any(entityType =>
                                entityType.IsGenericType
                                    ? entityType.IsAssignableFromGenericType(type)
                                    : entityType.IsAssignableFrom(type)
        );

    #endregion
}