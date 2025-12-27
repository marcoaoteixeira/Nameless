using System.ComponentModel;
using System.Data;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options;

public sealed record ConnectionSettings : SettingsBase {
    [Description(description: "connection.provider")]
    public string? Provider { get; set; }

    [Description(description: "connection.driver_class")]
    public string? DriverClass { get; set; } = "NHibernate.Driver.SQLite20Driver";

    [Description(description: "connection.connection_string")]
    public string? ConnectionString { get; set; } = "Data Source=:memory:";

    [Description(description: "connection.connection_string_name")]
    public string? ConnectionStringName { get; set; }

    [Description(description: "connection.isolation")]
    public IsolationLevel? Isolation { get; set; }

    [Description(description: "connection.release_mode")]
    public ReleaseMode? ReleaseMode { get; set; }
}