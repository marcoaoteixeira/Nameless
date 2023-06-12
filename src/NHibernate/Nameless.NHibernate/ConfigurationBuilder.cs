﻿using Microsoft.Extensions.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.NHibernate {

    public sealed class ConfigurationBuilder : IConfigurationBuilder {

        #region Private Read-Only Fields

        private readonly NHibernateOptions _options;

        #endregion

        #region Public Constructors

        public ConfigurationBuilder(IOptions<NHibernateOptions> options) {
            _options = options.Value ?? NHibernateOptions.Default;
        }

        #endregion

        #region Private Static Methods

        private static bool IsMappingType(Type? type) {
            if (type == default || type.IsAbstract || type.IsInterface) {
                return false;
            }

            return typeof(ClassMapping<>).IsAssignableFromGenericType(type)
                || typeof(JoinedSubclassMapping<>).IsAssignableFromGenericType(type)
                || typeof(SubclassMapping<>).IsAssignableFromGenericType(type)
                || typeof(UnionSubclassMapping<>).IsAssignableFromGenericType(type);
        }

        #endregion

        #region IConfigurationBuilder Members

        public Configuration Build() {
            var configuration = new Configuration();
            configuration.SetProperties(_options.ToDictionary());

            var entityBaseTypes = _options.EntityRootTypes.Select(Type.GetType).ToArray();
            var modelInspector = new ModelInspector(entityBaseTypes!);
            var modelMapper = new ModelMapper(modelInspector);

            var mappingTypes = _options.MappingTypes
                .Select(Type.GetType)
                .Where(IsMappingType)
                .ToArray();
            modelMapper.AddMappings(mappingTypes);

            configuration.AddDeserializedMapping(modelMapper.CompileMappingForAllExplicitlyAddedEntities(), default);

            return configuration;
        }

        #endregion
    }
}