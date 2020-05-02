using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Used to define the classes that will be mapped to DB schemas.
    /// </summary>
    public sealed class ClassMapper {

        #region Private Read-Only Fields

        private readonly IModelInspector _modelInspector;
        private readonly Type[] _mappingTypes;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ClassMapper"/>.
        /// </summary>
        /// <param name="modelInspector">Model inspector.</param>
        /// <param name="mappingTypes">An array of <see cref="Type"/> that define all mapping types.</param>
        public ClassMapper (IModelInspector modelInspector, params Type[] mappingTypes) {
            Prevent.ParameterNull (modelInspector, nameof (modelInspector));

            _modelInspector = modelInspector;
            _mappingTypes = mappingTypes.Where (IsMappingType).ToArray ();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Maps to configuration
        /// </summary>
        /// <param name="configuration">An instance of <see cref="global::NHibernate.Cfg.Configuration"/>.</param>
        public void Map (global::NHibernate.Cfg.Configuration configuration) {
            Prevent.ParameterNull (configuration, nameof (configuration));

            if (_mappingTypes.IsNullOrEmpty ()) { return; }

            var mapper = new ModelMapper (_modelInspector);

            mapper.AddMappings (_mappingTypes);

            configuration.AddDeserializedMapping (
                mappingDocument: mapper.CompileMappingForAllExplicitlyAddedEntities (),
                documentFileName: null);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Verifies if the type is a mapping type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if is a mapping type; otherwise, <c>false</c>.</returns>
        public static bool IsMappingType (Type type) {
            return type != null &&
                !type.IsAbstract &&
                !type.IsInterface &&
                (
                    type.IsAssignableToGenericType (typeof (ClassMapping<>)) ||
                    type.IsAssignableToGenericType (typeof (JoinedSubclassMapping<>)) ||
                    type.IsAssignableToGenericType (typeof (SubclassMapping<>)) ||
                    type.IsAssignableToGenericType (typeof (UnionSubclassMapping<>))
                );
        }

        #endregion
    }
}