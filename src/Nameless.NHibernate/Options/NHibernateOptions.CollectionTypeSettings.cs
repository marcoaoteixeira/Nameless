using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record CollectionTypeSettings : SettingsBase {
    [Description("collectiontype.factory_class")]
    public string? FactoryClass { get; set; }
}