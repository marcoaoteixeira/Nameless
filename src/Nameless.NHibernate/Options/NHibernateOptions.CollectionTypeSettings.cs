using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record CollectionTypeSettings : SettingsBase {
    [Description(description: "collectiontype.factory_class")]
    public string? FactoryClass { get; set; }
}