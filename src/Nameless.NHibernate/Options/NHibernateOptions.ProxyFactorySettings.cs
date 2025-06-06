using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record ProxyFactorySettings : SettingsBase {
    [Description("proxyfactory.factory_class")]
    public string? FactoryClass { get; set; }
}