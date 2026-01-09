using System.ComponentModel;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options;

public record CommonSettings : SettingsBase {
    [Description(description: "dialect")]
    public string? Dialect { get; set; } = "NHibernate.Dialect.SQLiteDialect";

    [Description(description: "default_catalog")]
    public string? DefaultCatalog { get; set; }

    [Description(description: "default_schema")]
    public string? DefaultSchema { get; set; }

    [Description(description: "max_fetch_depth")]
    public int? MaxFetchDepth { get; set; }

    [Description(description: "sql_exception_converter")]
    public string? SqlExceptionConverter { get; set; }

    [Description(description: "show_sql")]
    public bool? ShowSql { get; set; } = true;

    [Description(description: "format_sql")]
    public bool? FormatSql { get; set; }

    [Description(description: "use_sql_comments")]
    public bool? UseSqlComments { get; set; }

    [Description(description: "use_proxy_validator")]
    public bool? UseProxyValidator { get; set; }

    [Description(description: "default_flush_mode")]
    public FlushMode? DefaultFlushMode { get; set; }

    [Description(description: "default_batch_fetch_size")]
    public int? DefaultBatchFetchSize { get; set; }

    [Description(description: "current_session_context_class")]
    public string? CurrentSessionContextClass { get; set; }

    [Description(description: "generate_statistics")]
    public bool? GenerateStatistics { get; set; }

    [Description(description: "track_session_id")]
    public bool? TrackSessionID { get; set; }

    [Description(description: "nhibernate-logger")]
    public string? NHibernateLogger { get; set; }

    [Description(description: "prepare_sql")]
    public bool? PrepareSql { get; set; }

    [Description(description: "command_timeout")]
    public int? CommandTimeout { get; set; }

    [Description(description: "order_inserts")]
    public bool? OrderInserts { get; set; }

    [Description(description: "order_updates")]
    public bool? OrderUpdates { get; set; }

    [Description(description: "id.optimizer.pooled.prefer_lo")]
    public bool? IDOptimizerPooledPreferLo { get; set; }

    [Description(description: "sql_types.keep_datetime")]
    public bool? SqlTypesKeepDateTime { get; set; }
}