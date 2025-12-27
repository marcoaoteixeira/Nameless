using System.ComponentModel;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options;

public sealed record HbmToDdlSettings : SettingsBase {
    [Description(description: "hbm2ddl.auto")]
    public HbmToDdlAuto? Auto { get; set; }

    [Description(description: "hbm2ddl.keywords")]
    public HbmToDdlKeyword? Keywords { get; set; }
}