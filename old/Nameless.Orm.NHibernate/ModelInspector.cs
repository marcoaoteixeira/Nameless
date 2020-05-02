using System;
using System.Linq;
using NHibernate.Mapping.ByCode;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Default implementation of <see cref="ExplicitlyDeclaredModel"/>.
    /// </summary>
    public class ModelInspector : ExplicitlyDeclaredModel {

        #region Private Read-Only Fields

        private readonly Type[] _entityTypes;

        #endregion

        #region Public Constructrs

        /// <summary>
        /// Initializes a new instance of <see cref="ModelInspector"/>
        /// </summary>
        /// <param name="entityTypes">An array of entity types.</param>
        public ModelInspector (Type[] entityTypes) {
            Prevent.ParameterNull (entityTypes, nameof (entityTypes));

            _entityTypes = entityTypes;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Verifies if the specified type implements <see cref="IModelInspector"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if implements; otherwise, <c>false</c>.</returns>
        public static bool IsModelInspector (Type type) {
            return type != null &&
                !type.IsAbstract &&
                !type.IsInterface &&
                typeof (IModelInspector).IsAssignableFrom (type);
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override bool IsEntity (Type type) {
            return typeof (EntityBase).IsAssignableFrom (type) ||
                typeof (EntityBase).IsAssignableFrom (type.BaseType) ||
                _entityTypes.Any (_ => _.IsAssignableFrom (type) ||
                    _.IsAssignableFrom (type.BaseType)
                );
        }

        #endregion
    }
}