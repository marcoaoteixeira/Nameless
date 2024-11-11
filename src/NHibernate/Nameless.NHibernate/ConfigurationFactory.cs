using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate;

public sealed class ConfigurationFactory : IConfigurationFactory {
    private readonly NHibernateOptions _options;
    private readonly ILogger<ConfigurationFactory> _logger;

    public ConfigurationFactory(IOptions<NHibernateOptions> options, ILogger<ConfigurationFactory> logger) {
        _options = Prevent.Argument.Null(options).Value;
        _logger = Prevent.Argument.Null(logger);
    }

    public Configuration CreateConfiguration() {
        var configuration = new Configuration();
        configuration.SetProperties(_options.ToDictionary());

        var entityTypes = LoadTypes(_options.TypeMappingSettings.Entities);
        var modelInspector = new ModelInspector(entityTypes.ToArray());
        var modelMapper = new ModelMapper(modelInspector);

        var classMappingTypes = LoadTypes(_options.TypeMappingSettings.ClassMappings);
        modelMapper.AddMappings(classMappingTypes);

        var mappingDocument = modelMapper
            .CompileMappingForAllExplicitlyAddedEntities();

        configuration.AddDeserializedMapping(mappingDocument: mappingDocument,
                                             documentFileName: null);

        return configuration;
    }

    private IEnumerable<Type> LoadTypes(string[] types) {
        foreach (var type in types) {
            Type result;
            
            try { result = Type.GetType(type) ?? typeof(void); }
            catch (Exception ex) {
                _logger.ErrorOnLoadType(type, ex);
                continue;
            }

            if (result == typeof(void)) {
                _logger.LoadTypeResolveToVoid(type);
                continue;
            }

            yield return result;
        }
    }
}