using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record LinqToHqlSettings : SettingsBase {
    [Description(description: "linqtohql.generatorsregistry")]
    public string? GeneratorsRegistry { get; set; }
}