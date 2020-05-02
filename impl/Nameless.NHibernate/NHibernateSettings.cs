using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using NHEnvironment = NHibernate.Cfg.Environment;

namespace Nameless.NHibernate {
    public sealed class NHibernateSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "dialect". The class name of a
        /// NHibernate <see cref="global::NHibernate.Dialect.Dialect" />.
        /// Enables certain platform dependent features. Default value is
        /// "NHibernate.Dialect.SQLiteDialect,NHibernate".
        /// </summary>
        public string Dialect { get; set; } = $"{typeof (global::NHibernate.Dialect.SQLiteDialect).FullName},{typeof (global::NHibernate.Dialect.SQLiteDialect).Assembly.GetName ().Name}";

        /// <summary>
        /// Gets or sets the property for "default_catalog". Qualify
        /// unqualified table names with the given catalog name in generated
        /// SQL.
        /// </summary>
        public string DefaultCatalog { get; set; }

        /// <summary>
        /// Gets or sets the property for "default_schema". Qualify unqualified
        /// table names with the given schema/table-space in generated SQL.
        /// </summary>
        public string DefaultSchema { get; set; }

        public Ado Ado { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the configuration section to a NHibernate property dictionary.
        /// </summary>
        /// <returns>An instance of <see cref="IDictionary{String, String}"/></returns>
        public IDictionary<string, string> ToPropertiesDictionary () {
            var result = new Dictionary<string, string> {
                    [NHEnvironment.DefaultSchema] = DbSchemaName,
                    [NHEnvironment.BatchSize] = BatchSize.ToString (),
                    [NHEnvironment.CommandTimeout] = CommandTimeout.ToString (),
                    [NHEnvironment.ConnectionDriver] = Driver,
                    [NHEnvironment.Dialect] = Dialect,
                    [NHEnvironment.FormatSql] = LogFormattedSql.ToString (),
                    [NHEnvironment.Hbm2ddlKeyWords] = KeywordsAutoImport,
                    [NHEnvironment.Isolation] = IsolationLevel.ToString (),
                    [NHEnvironment.QuerySubstitutions] = QueryLanguageSubstituitions
                };

            result[NHEnvironment.ReleaseConnections] = ConnectionReleaseMode
            switch {
                ConnectionReleaseMode.AfterTransaction => "after_transaction",
                ConnectionReleaseMode.OnClose => "on_close",
                _ => "auto",
            };

            result[NHEnvironment.ShowSql] = LogSqlInConsole.ToString ();
            result[NHEnvironment.UseSqlComments] = OutputSqlCommand.ToString ();
            result[NHEnvironment.ConnectionString] = ConnectionString;
            result[NHEnvironment.Hbm2ddlAuto] = SchemaChangeAction;

            return result;
        }

        #endregion
    }

    public sealed class Ado {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "connection.provider". Default value
        /// is "NHibernate.Connection.DriverConnectionProvider".
        /// </summary>
        public string Provider { get; set; } = typeof (global::NHibernate.Connection.DriverConnectionProvider).FullName;

        /// <summary>
        /// Gets or sets the property for "connection.driver_class". This is
        /// usually not needed, most of the time the dialect will take care of
        /// setting the <see cref="global::NHibernate.Driver.IDriver" /> using
        /// a sensible default.
        /// </summary>
        public string DriverClass { get; set; }

        /// <summary>
        /// Gets or sets the property for "connection.connection_string".
        /// Default value is "Data Source=:memory:; Version=3; Page Size=8192;"
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=:memory:; Version=3; Page Size=8192;";

        /// <summary>
        /// Gets or sets the property for "connection.isolation". Set the
        /// ADO.NET transaction isolation level. Check
        /// <see cref="IsolationLevel" /> for meaningful values and the
        /// database's documentation to ensure that level is supported. Default
        /// value is <see cref="IsolationLevel.Unspecified" />.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Unspecified;

        /// <summary>
        /// Gets or sets the property for "connection.release_mode". Specify
        /// when NHibernate should release ADO.NET connections. Default value
        /// is <see cref="global::NHibernate.ConnectionReleaseMode.OnClose" />
        /// </summary>
        public global::NHibernate.ConnectionReleaseMode ConnectionReleaseMode { get; set; } = global::NHibernate.ConnectionReleaseMode.OnClose;

        /// <summary>
        /// Gets or sets the property for "prepare_sql". Specify to prepare
        /// DbCommands generated by NHibernate. Default value is <c>false</c>.
        /// </summary>
        public bool Prepare { get; set; }

        /// <summary>
        /// Gets or sets the property for "command_timeout". Specify the
        /// default timeout in seconds of DbCommands generated by NHibernate.
        /// Negative values disable it. Default value is 15 seconds.
        /// </summary>
        public int CommandTimeout { get; set; } = 15;

        /// <summary>
        /// Gets or sets the property for "ado.batch_size". Specify the batch
        /// size to use when batching update statements. Setting this to 0
        /// (the default) disables the functionality. Default value is 8
        /// seconds.
        /// </summary>
        public int BatchSize { get; set; } = 8;

        /// <summary>
        /// Gets or sets the property for "order_inserts". Enable ordering of
        /// insert statements for the purpose of more efficient batching.
        /// Defaults to <c>true</c> if batching is enabled, <c>false</c>
        /// otherwise.
        /// </summary>
        public bool? OrderInsert { get; set; }

        /// <summary>
        /// Gets or sets the property for "order_updates". Enable ordering of
        /// update statements for the purpose of more efficient batching.
        /// Defaults to <c>true</c> if batching is enabled, <c>false</c>
        /// otherwise.
        /// </summary>
        public bool? OrderUpdate { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.batch_versioned_data". If
        /// batching is enabled, specify that versioned data can also be
        /// batched. Requires a dialect which batcher correctly returns rows
        /// count. Defaults to <c>false</c>.
        /// </summary>
        public bool BatchVersionedData { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.factory_class". This is
        /// usually not needed, most of the time the driver will take care of
        /// setting the <see cref="NHibernate.AdoNet.IBatcherFactory" /> 
        /// using a sensible default according to the database capabilities.
        /// </summary>
        public string BatchFactory { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.wrap_result_sets". Some
        /// database vendor data reader implementation have inefficient
        /// columnName-to-columnIndex resolution. Enabling this setting allows
        /// to wrap them in a data reader that will cache those resolutions. 
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool WrapResultSet { get; set; }

        #endregion
    }

    public sealed class Cache {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "cache.use_second_level_cache".
        /// Enable the second level cache. Requires specifying a
        /// <see cref="SecondLevelCacheProvider" />. Default value is
        /// <c>false</c>
        /// </summary>
        public bool UseSecondLevelCache { get; set; }

        /// <summary>
        /// Gets or sets the property for "cache.provider_class". The class
        /// name of a <see cref="global::NHibernate.Cache.ICacheProvider" />
        /// implementation.
        /// </summary>
        public string SecondLevelCacheProvider { get; set; }

        /// <summary>
        /// Gets or sets the property for "cache.use_minimal_puts". Optimize
        /// second-level cache operation to minimize writes, at the cost of
        /// more frequent reads (useful for clustered caches).
        /// Default value is <c>false</c>.
        /// </summary>
        public bool UseMinimalPuts { get; set; }

        /// <summary>
        /// Gets or sets the property for "cache.use_query_cache". Enable the
        /// query cache, individual queries still have to be set cacheable.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool UseQueryCache { get; set; }

        /// <summary>
        /// Gets or sets the property for "". The class name of a custom
        /// <see cref="global::NHibernate.Cache.IQueryCacheFactory" />
        /// implementation. Default value is the built-in
        /// <see cref="global::NHibernate.Cache.StandardQueryCacheFactory" />.
        /// </summary>
        public string QueryCacheFactory { get; set; } = typeof (global::NHibernate.Cache.StandardQueryCacheFactory).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "cache.region_prefix". A prefix to
        /// use for second-level cache region names.
        /// </summary>
        public string RegionPrefix { get; set; }

        /// <summary>
        /// Gets or sets the property for "cache.default_expiration". The
        /// default expiration delay, in seconds, for cached entries, for
        /// providers supporting this setting.
        /// </summary>
        public int? DefaultExpiration { get; set; }

        #endregion
    }

    public sealed class Query {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "max_fetch_depth". Set a maximum
        /// "depth" for the outer join fetch tree for single-ended associations
        /// (one-to-one, many-to-one). Value 0 disables default outer
        /// join fetching. Recommended values between 0 and 3. Default value
        /// is 1.
        /// </summary>
        public int MaxFetchDepth { get; set; } = 1;

        /// <summary>
        /// Gets or sets the property for "query.substitutions". Mapping from
        /// tokens in NHibernate queries to SQL tokens (tokens might be
        /// function or literal names, for example). Default value is
        /// "true 1, false 0, yes 'Y', no 'N'";
        /// </summary>
        public string Substitutions { get; set; } = "true 1, false 0, yes 'Y', no 'N'";

        /// <summary>
        /// Gets or sets the property for "query.default_cast_length". Set
        /// the default length used in casting when the target type is length
        /// bound and does not specify it. Default value is 4000, automatically
        /// trimmed down according to dialect type registration.
        /// </summary>
        public int DefaultCastLength { get; set; } = 4000;

        /// <summary>
        /// Gets or sets the property for "query.default_cast_precision". Set
        /// the default precision used in casting when the target type is
        /// decimal and does not specify it. Default value is 28, automatically
        /// trimmed down according to dialect type registration.
        /// </summary>
        public int DefaultCastPrecision { get; set; } = 28;

        /// <summary>
        /// Gets or sets the property for "query.default_cast_scale". Set the
        /// default scale used in casting when the target type is decimal and
        /// does not specify it. Default value is 10, automatically trimmed
        /// down according to dialect type registration.
        /// </summary>
        public int DefaultCastScale { get; set; } = 10;

        /// <summary>
        /// Gets or sets the property for "query.startup_check". Should named
        /// queries be checked during startup. Default value is <c>true</c>.
        /// </summary>
        public bool StartUpCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets the property "query.factory_class". The class name of
        /// a custom <see cref="global::NHibernate.Hql.IQueryTranslatorFactory" />
        /// implementation (HQL query parser factory). Default value is the
        /// built-in <see cref="global::NHibernate.Hql.Ast.ANTLR.ASTQueryTranslatorFactory" />.
        /// </summary>
        public string Factory { get; set; } = typeof (global::NHibernate.Hql.Ast.ANTLR.ASTQueryTranslatorFactory).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "query.linq_provider_class". The
        /// class name of a custom <see cref="global::NHibernate.Linq.INhQueryProvider" />
        /// implementation (LINQ provider). Default value is the built-in
        /// <see cref="global::NHibernate.Linq.DefaultQueryProvider" />.
        /// </summary>
        public string LinqProvider { get; set; } = typeof (global::NHibernate.Linq.DefaultQueryProvider).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "query.query_model_rewriter_factory".
        /// The class name of a custom <see cref="global::NHibernate.Linq.Visitors.IQueryModelRewriterFactory" />
        /// implementation (LINQ query model rewriter factory). Default value
        /// is <c>null</c> (no rewriter).
        /// </summary>
        public string QueryModelRewriterFactory { get; set; }

        #endregion
    }

    public sealed class Output {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "show_sql". Write all SQL statements
        /// to console. Default value is <c>false</c>.
        /// </summary>
        public bool ShowSQL { get; set; }

        /// <summary>
        /// Gets or sets the property for "format_sql". Log formatted SQL.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool FormatSQL { get; set; }

        /// <summary>
        /// Gets or sets the property for "use_sql_comments". Generate SQL with
        /// comments. Default value is <c>false</c>.
        /// </summary>
        public bool UseSQLComments { get; set; }

        #endregion
    }

    public sealed class Mapping {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "hbm2ddl.auto". Automatically export
        /// schema DDL to the database when the
        /// <see cref="global::NHibernate.ISessionFactory" /> is created. With
        /// create-drop, the database schema will be dropped when the
        /// <see cref="global::NHibernate.ISessionFactory" /> is closed
        /// explicitly. Default value is "update".
        /// </summary>
        public string Hbm2DllAuto { get; set; } = "update";

        /// <summary>
        /// Gets or sets the property for "hbm2ddl.keywords". Automatically
        /// import reserved/keywords from the database when the
        /// <see cref="global::NHibernate.ISessionFactory" /> is created.
        /// <para><c>none</c> : disable any operation regarding RDBMS KeyWords
        /// (the default).</para>
        /// <para><c>keywords</c> : imports all RDBMS KeyWords where the
        /// Dialect can provide the implementation of
        /// <see cref="global::NHibernate.Dialect.Schema.IDataBaseSchema" />.</para>
        /// <para><c>auto-quote</c> : imports all RDBMS KeyWords and auto-quote
        /// all table-names/column-names.</para>
        /// </summary>
        public string Hbm2DllKeywords { get; set; } = "none";

        #endregion
    }
}