using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public record ProxyFactorySettings : SettingsBase {
    [Description(description: "proxyfactory.factory_class")]
    public string? FactoryClass { get; set; }
}