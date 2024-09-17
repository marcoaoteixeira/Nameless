using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateProxyFactoryOptions : NHibernateOptionsBase {
    [Description("proxyfactory.factory_class")]
    public string? FactoryClass { get; set; }
}