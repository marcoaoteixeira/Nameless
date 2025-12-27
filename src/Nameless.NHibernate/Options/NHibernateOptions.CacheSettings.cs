using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record CacheSettings : SettingsBase {
    [Description(description: "cache.use_second_level_cache")]
    public bool? UseSecondLevelCache { get; set; }

    [Description(description: "cache.provider_class")]
    public string? ProviderClass { get; set; }

    [Description(description: "cache.use_minimal_puts")]
    public bool? UseMinimalPuts { get; set; }

    [Description(description: "cache.query_cache_factory")]
    public string? QueryCacheFactory { get; set; }

    [Description(description: "cache.region_prefix")]
    public string? RegionPrefix { get; set; }

    [Description(description: "cache.default_expiration")]
    public int? DefaultExpiration { get; set; }
}