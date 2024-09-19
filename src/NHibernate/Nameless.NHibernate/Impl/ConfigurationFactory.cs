using Microsoft.Extensions.Options;
using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Impl;

public sealed class ConfigurationFactory : IConfigurationFactory {
    private readonly NHibernateOptions _options;
    private readonly Type[] _entityTypes;
    private readonly Type[] _classMappingTypes;

    public ConfigurationFactory(IOptions<NHibernateOptions> options, Type[] entityTypes, Type[] classMappingTypes) {
        _entityTypes = Prevent.Argument.Null(entityTypes);
        _classMappingTypes = Prevent.Argument.Null(classMappingTypes);
        _options = Prevent.Argument.Null(options).Value;
    }

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
}