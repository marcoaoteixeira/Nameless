using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Impl {
    public sealed class ConfigurationFactory : IConfigurationFactory {
        #region Private Read-Only Fields

        private readonly NHibernateOptions _options;
        private readonly Type[] _entityTypes;
        private readonly Type[] _classMappingTypes;

        #endregion

        #region Public Constructors

        public ConfigurationFactory(NHibernateOptions options, Type[] entityTypes, Type[] classMappingTypes) {
            _entityTypes = Guard.Against.Null(entityTypes, nameof(entityTypes));
            _classMappingTypes = Guard.Against.Null(classMappingTypes, nameof(classMappingTypes));
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region IConfigurationFactory Members

        public Configuration CreateConfiguration() {
            var configuration = new Configuration();
            configuration.SetProperties(_options.ToDictionary());

            var modelInspector = new ModelInspector(_entityTypes);
            var modelMapper = new ModelMapper(modelInspector);

            modelMapper.AddMappings(_classMappingTypes);

            var mappingDocument = modelMapper
                .CompileMappingForAllExplicitlyAddedEntities();

            configuration.AddDeserializedMapping(mappingDocument: mappingDocument,
                                                 documentFileName: null);

            return configuration;
        }

        #endregion
    }
}
