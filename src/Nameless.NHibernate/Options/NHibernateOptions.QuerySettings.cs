using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record QuerySettings : SettingsBase {
    [Description(description: "query.substitutions")]
    public string? Substitutions { get; set; } = "true=1;false=0";

    [Description(description: "query.default_cast_length")]
    public int? DefaultCastLength { get; set; }

    [Description(description: "query.default_cast_precision")]
    public int? DefaultCastPrecision { get; set; }

    [Description(description: "query.default_cast_scale")]
    public int? DefaultCastScale { get; set; }

    [Description(description: "query.startup_check")]
    public bool? StartupCheck { get; set; }

    [Description(description: "query.factory_class")]
    public string? FactoryClass { get; set; }

    [Description(description: "query.linq_provider_class")]
    public string? LinqProviderClass { get; set; }

    [Description(description: "query.query_model_rewriter_factory")]
    public string? QueryModelRewriterFactory { get; set; }
}