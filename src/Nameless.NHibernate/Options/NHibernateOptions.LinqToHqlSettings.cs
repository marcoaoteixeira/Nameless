using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public record LinqToHqlSettings : SettingsBase {
    [Description(description: "linqtohql.generatorsregistry")]
    public string? GeneratorsRegistry { get; set; }
}