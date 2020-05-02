using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHEnv = NHibernate.Cfg.Environment;

namespace Nameless.Orm.NHibernate {
    public class NHibernateSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "dialect".
        /// </summary>
        public string Dialect { get; set; } = "NHibernate.Dialect.SQLiteDialect,NHibernate";

        /// <summary>
        /// Gets or sets the property for "connection.driver_class".
        /// </summary>
        public string Driver { get; set; } = "NHibernate.Driver.SQLite20Driver,NHibernate";

        /// <summary>
        /// Gets or sets the property for "hbm2ddl.keywords"
        /// </summary>
        public string KeywordsAutoImport { get; set; } = "auto-quote";

        /// <summary>
        /// Gets or sets the property for "connection.isolation".
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Unspecified;

        /// <summary>
        /// Gets or sets the property for "command_timeout".
        /// </summary>
        public int Timeout { get; set; } = 5;

        /// <summary>
        /// Gets or sets the property for "format_sql".
        /// </summary>
        public bool LogFormattedSql { get; set; } = true;

        /// <summary>
        /// Gets or sets the property for "show_sql".
        /// </summary>
        public bool LogSqlInConsole { get; set; } = true;

        /// <summary>
        /// Gets or sets the property for "use_sql_comments".
        /// </summary>
        public bool AutoCommentSql { get; set; } = true;

        /// <summary>
        /// Gets or sets the property for "ado.batch_size".
        /// </summary>
        public int BatchSize { get; set; } = 16;

        /// <summary>
        /// Gets or sets the property for "query.substitutions".
        /// </summary>
        public string HqlToSqlSubstitutions { get; set; } = "true 1, false 0, yes 'Y', no 'N'";

        /// <summary>
        /// Gets or sets the property for "connection.release_mode".
        /// </summary>
        public ConnectionReleaseMode ConnectionReleaseMode { get; set; } = ConnectionReleaseMode.OnClose;

        /// <summary>
        /// Gets or sets the property for "connection.connection_string".
        /// </summary>
        public string ConnectionString { get; set; } = @"Data Source=:memory:; Version=3; Page Size=8192;";

        /// <summary>
        /// Gets or sets the property for "hbm2dll.auto".
        /// </summary>
		public string Hbm2DllAuto { get; set; } = "update";

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the configuration section to a NHibernate property dictionary.
        /// </summary>
        /// <returns>An instance of <see cref="IDictionary{String, String}"/></returns>
        public IDictionary<string, string> ToPropertiesDictionary () {
            var result = new Dictionary<string, string> {
                [NHEnv.BatchSize] = BatchSize.ToString (),
                [NHEnv.CommandTimeout] = Timeout.ToString (),
                [NHEnv.ConnectionDriver] = Driver,
                [NHEnv.Dialect] = Dialect,
                [NHEnv.FormatSql] = LogFormattedSql.ToString (),
                [NHEnv.Hbm2ddlKeyWords] = KeywordsAutoImport,
                [NHEnv.Isolation] = IsolationLevel.ToString (),
                [NHEnv.QuerySubstitutions] = HqlToSqlSubstitutions
            };

            switch (ConnectionReleaseMode) {
                case ConnectionReleaseMode.AfterStatement:
                    result[NHEnv.ReleaseConnections] = "auto";
                    break;

                case ConnectionReleaseMode.AfterTransaction:
                    result[NHEnv.ReleaseConnections] = "after_transaction";
                    break;

                case ConnectionReleaseMode.OnClose:
                    result[NHEnv.ReleaseConnections] = "on_close";
                    break;
            }

            result[NHEnv.ShowSql] = LogSqlInConsole.ToString ();
            result[NHEnv.UseSqlComments] = AutoCommentSql.ToString ();
            result[NHEnv.ConnectionString] = ConnectionString;
            result[NHEnv.Hbm2ddlAuto] = Hbm2DllAuto;

            return result;
        }

        #endregion
    }
}