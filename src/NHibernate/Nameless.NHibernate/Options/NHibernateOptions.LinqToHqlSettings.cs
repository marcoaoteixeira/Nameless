using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record LinqToHqlSettings : ConfigurationNode {
    [Description("linqtohql.generatorsregistry")]
    public string? GeneratorsRegistry { get; set; }
}