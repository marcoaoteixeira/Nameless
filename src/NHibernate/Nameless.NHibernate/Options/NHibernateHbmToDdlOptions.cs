using System.ComponentModel;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options;

public sealed record NHibernateHbmToDdlOptions : NHibernateOptionsBase {
    [Description("hbm2ddl.auto")]
    public HbmToDdlAuto? Auto { get; set; }

    [Description("hbm2ddl.keywords")]
    public HbmToDdlKeyword? Keywords { get; set; }
}