using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateCacheOptions : NHibernateOptionsBase {
    [Description("cache.use_second_level_cache")]
    public bool? UseSecondLevelCache { get; set; }

    [Description("cache.provider_class")]
    public string? ProviderClass { get; set; }

    [Description("cache.use_minimal_puts")]
    public bool? UseMinimalPuts { get; set; }

    [Description("cache.query_cache_factory")]
    public string? QueryCacheFactory { get; set; }

    [Description("cache.region_prefix")]
    public string? RegionPrefix { get; set; }

    [Description("cache.default_expiration")]
    public int? DefaultExpiration { get; set; }
}