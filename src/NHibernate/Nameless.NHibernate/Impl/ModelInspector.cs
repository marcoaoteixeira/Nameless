using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate {
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
            _entityTypes = Guard.Against.NullOrEmpty(entityTypes, nameof(entityTypes));
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override bool IsEntity(Type type)
            => _entityTypes.Any(_ =>
                _.IsGenericType
                    ? _.IsAssignableFromGenericType(type)
                    : _.IsAssignableFrom(type)
            );

        #endregion
    }
}
