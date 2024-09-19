using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateCollectionTypeOptions : NHibernateOptionsBase {
    [Description("collectiontype.factory_class")]
    public string? FactoryClass { get; set; }
}