using Microsoft.Extensions.Options;
using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IConfigurationFactory"/>.
/// </summary>
public sealed class ConfigurationFactory : IConfigurationFactory {
    private readonly IOptions<NHibernateOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationFactory"/>.
    /// </summary>
    /// <param name="options"> The NHibernate options to configure the factory with.</param>
    public ConfigurationFactory(IOptions<NHibernateOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    public Configuration CreateConfiguration() {
        var options = _options.Value;
        var configuration = new Configuration().SetProperties(options.ToDictionary());
        var modelInspector = new ModelInspector(options.Entities.ToArray());
        var modelMapper = new ModelMapper(modelInspector);

        modelMapper.AddMappings(options.Mappings);

        var mappingDocument = modelMapper.CompileMappingForAllExplicitlyAddedEntities();

        configuration.AddDeserializedMapping(mappingDocument, documentFileName: null);

        return configuration;
    }
}