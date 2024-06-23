using System.ComponentModel;
using Nameless.NHibernate.Objects;

namespace Nameless.NHibernate.Options {
    public sealed class NHibernateCommonOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("dialect")]
        public string? Dialect { get; set; } = "NHibernate.Dialect.SQLiteDialect";

        [Description("default_catalog")]
        public string? DefaultCatalog { get; set; }

        [Description("default_schema")]
        public string? DefaultSchema { get; set; }

        [Description("max_fetch_depth")]
        public int? MaxFetchDepth { get; set; }

        [Description("sql_exception_converter")]
        public string? SqlExceptionConverter { get; set; }

        [Description("show_sql")]
        public bool? ShowSql { get; set; } = true;

        [Description("format_sql")]
        public bool? FormatSql { get; set; }

        [Description("use_sql_comments")]
        public bool? UseSqlComments { get; set; }

        [Description("use_proxy_validator")]
        public bool? UseProxyValidator { get; set; }

        [Description("default_flush_mode")]
        public FlushMode? DefaultFlushMode { get; set; }

        [Description("default_batch_fetch_size")]
        public int? DefaultBatchFetchSize { get; set; }

        [Description("current_session_context_class")]
        public string? CurrentSessionContextClass { get; set; }

        [Description("generate_statistics")]
        public bool? GenerateStatistics { get; set; }

        [Description("track_session_id")]
        public bool? TrackSessionID { get; set; }

        [Description("nhibernate-logger")]
        public string? NHibernateLogger { get; set; }

        [Description("prepare_sql")]
        public bool? PrepareSql { get; set; }

        [Description("command_timeout")]
        public int? CommandTimeout { get; set; }

        [Description("order_inserts")]
        public bool? OrderInserts { get; set; }

        [Description("order_updates")]
        public bool? OrderUpdates { get; set; }

        [Description("id.optimizer.pooled.prefer_lo")]
        public bool? IDOptimizerPooledPreferLo { get; set; }

        [Description("sql_types.keep_datetime")]
        public bool? SqlTypesKeepDateTime { get; set; }

        #endregion
    }
}
