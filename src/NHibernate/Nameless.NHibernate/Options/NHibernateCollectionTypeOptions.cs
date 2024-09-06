using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateCollectionTypeOptions : NHibernateOptionsBase {
    #region Public Properties

    [Description("collectiontype.factory_class")]
    public string? FactoryClass { get; set; }

    #endregion
}