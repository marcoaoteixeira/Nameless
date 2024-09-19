using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateLinqToHqlOptions : NHibernateOptionsBase {
    [Description("linqtohql.generatorsregistry")]
    public string? GeneratorsRegistry { get; set; }
}