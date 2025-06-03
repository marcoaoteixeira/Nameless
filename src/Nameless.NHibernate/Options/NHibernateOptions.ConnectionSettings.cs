using System.ComponentModel;
using System.Data;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options;

public sealed record ConnectionSettings : SettingsBase {
    [Description("connection.provider")]
    public string? Provider { get; set; }

    [Description("connection.driver_class")]
    public string? DriverClass { get; set; } = "NHibernate.Driver.SQLite20Driver";

    [Description("connection.connection_string")]
    public string? ConnectionString { get; set; } = "Data Source=:memory:";

    [Description("connection.connection_string_name")]
    public string? ConnectionStringName { get; set; }

    [Description("connection.isolation")]
    public IsolationLevel? Isolation { get; set; }

    [Description("connection.release_mode")]
    public ReleaseMode? ReleaseMode { get; set; }
}