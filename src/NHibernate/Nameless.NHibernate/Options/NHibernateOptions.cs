using System.Reflection;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateOptions {
    private SchemaExportSettings? _schemaExportSettings;
    public SchemaExportSettings SchemaExportSettings {
        get => _schemaExportSettings ?? new SchemaExportSettings();
        set => _schemaExportSettings = value;
    }

    private CommonSettings? _commonSettings;
    public CommonSettings CommonSettings {
        get => _commonSettings ?? new CommonSettings();
        set => _commonSettings = value;
    }

    private ConnectionSettings? _connectionSettings;
    public ConnectionSettings ConnectionSettings {
        get => _connectionSettings ?? new ConnectionSettings();
        set => _connectionSettings = value;
    }

    private AdoNetSettings? _adoNetSettings;
    public AdoNetSettings AdoNetSettings {
        get => _adoNetSettings ?? new AdoNetSettings();
        set => _adoNetSettings = value;
    }

    private CacheSettings? _cacheSettings;
    public CacheSettings CacheSettings {
        get => _cacheSettings ?? new CacheSettings();
        set => _cacheSettings = value;
    }

    private QuerySettings? _querySettings;
    public QuerySettings QuerySettings {
        get => _querySettings ?? new QuerySettings();
        set => _querySettings = value;
    }

    private LinqToHqlSettings? _linqToHqlSettings;
    public LinqToHqlSettings LinqToHqlSettings {
        get => _linqToHqlSettings ?? new LinqToHqlSettings();
        set => _linqToHqlSettings = value;
    }

    private HbmToDdlSettings? _hbmToDdlSettings;
    public HbmToDdlSettings HbmToDdlSettings {
        get => _hbmToDdlSettings ?? new HbmToDdlSettings();
        set => _hbmToDdlSettings = value;
    }

    private ProxyFactorySettings? _proxyFactorySettings;
    public ProxyFactorySettings ProxyFactorySettings {
        get => _proxyFactorySettings ?? new ProxyFactorySettings();
        set => _proxyFactorySettings = value;
    }

    private CollectionTypeSettings? _collectionTypeSettings;
    public CollectionTypeSettings CollectionTypeSettings {
        get => _collectionTypeSettings ?? new CollectionTypeSettings();
        set => _collectionTypeSettings = value;
    }

    private TransactionSettings? _transactionSettings;
    public TransactionSettings TransactionSettings {
        get => _transactionSettings ?? new TransactionSettings();
        set => _transactionSettings = value;
    }

    private SpecificSettings? _specificSettings;
    public SpecificSettings SpecificSettings {
        get => _specificSettings ?? new SpecificSettings();
        set => _specificSettings = value;
    }

    private TypeMappingSettings? _typeMappingSettings;
    public TypeMappingSettings TypeMappingSettings {
        get => _typeMappingSettings ?? new TypeMappingSettings();
        set => _typeMappingSettings = value;
    }

    public Dictionary<string, string> ToDictionary() {
        var configs = new List<KeyValuePair<string, string>>();

        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(property => typeof(ConfigurationNode).IsAssignableFrom(property.PropertyType));

        foreach (var property in properties) {
            if (property.GetValue(this) is not ConfigurationNode options) {
                continue;
            }

            configs.AddRange(options.GetConfigValues());
        }

        return new Dictionary<string, string>([.. configs]);
    }
}