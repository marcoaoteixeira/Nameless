using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record CollectionTypeSettings : ConfigurationNode {
    [Description("collectiontype.factory_class")]
    public string? FactoryClass { get; set; }
}