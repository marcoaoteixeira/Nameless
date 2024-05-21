using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.NHibernate.Impl {
    public sealed class ConfigurationFactory : IConfigurationFactory {
        #region Private Read-Only Fields

        private readonly NHibernateOptions _options;

        #endregion

        #region Public Constructors

        public ConfigurationFactory()
            : this(NHibernateOptions.Default) { }

        public ConfigurationFactory(NHibernateOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Static Methods

        private static bool IsMappingType(Type? type) {
            if (type is null || type.IsAbstract || type.IsInterface) {
                return false;
            }

            return typeof(ClassMapping<>).IsAssignableFromGenericType(type)
                || typeof(JoinedSubclassMapping<>).IsAssignableFromGenericType(type)
                || typeof(SubclassMapping<>).IsAssignableFromGenericType(type)
                || typeof(UnionSubclassMapping<>).IsAssignableFromGenericType(type);
        }

        #endregion

        #region IConfigurationFactory Members

        public Configuration CreateConfiguration() {
            var configuration = new Configuration();
            configuration.SetProperties(_options.ToDictionary());

            var entityRootTypes = _options.EntityRootTypes
                .Select(Type.GetType)
                .ToArray();
            var modelInspector = new ModelInspector(entityRootTypes!);
            var modelMapper = new ModelMapper(modelInspector);

            var mappings = _options.MappingTypes
                .Select(Type.GetType)
                .Where(IsMappingType)
                .ToArray();
            modelMapper.AddMappings(mappings);

            var mappingDocument = modelMapper
                .CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddDeserializedMapping(mappingDocument: mappingDocument,
                                                 documentFileName: null);

            return configuration;
        }

        #endregion
    }
}
