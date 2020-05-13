using System.Data;
using NHEnvironment = NHibernate.Cfg.Environment;

namespace Nameless.Persistence.NHibernate {
    public sealed class NHibernateSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the common settings.
        /// </summary>
        public CommonSettings Common { get; set; } = new CommonSettings ();

        /// <summary>
        /// Gets or sets the ADO settings.
        /// </summary>
        public AdoSettings Ado { get; set; } = new AdoSettings ();

        /// <summary>
        /// Gets or sets the cache settings.
        /// </summary>
        public CacheSettings Cache { get; set; } = new CacheSettings ();

        /// <summary>
        /// Gets or sets the query settings.
        /// </summary>
        public QuerySettings Query { get; set; } = new QuerySettings ();

        /// <summary>
        /// Gets or sets the output settings.
        /// </summary>
        public OutputSettings Output { get; set; } = new OutputSettings ();

        /// <summary>
        /// Gets or sets the mapping settings.
        /// </summary>
        public MappingSettings Mapping { get; set; } = new MappingSettings ();

        /// <summary>
        /// Gets or sets the proxy settings.
        /// </summary>
        public ProxySettings Proxy { get; set; } = new ProxySettings ();

        /// <summary>
        /// Gets or sets the transaction settings.
        /// </summary>
        public TransactionSettings Transaction { get; set; } = new TransactionSettings ();

        /// <summary>
        /// Gets or sets the database specific settings.
        /// </summary>
        public DatabaseSpecificSettings DatabaseSpecific { get; set; } = new DatabaseSpecificSettings ();

        #endregion

        #region Public Methods

        /// <summary>
        /// Arrange the NHibernate configuration object.
        /// </summary>
        /// <param name="configuration">The NHibernate configuration</param>
        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            (Common ?? new CommonSettings ()).SetConfigurationProperties (configuration);
            (Ado ?? new AdoSettings ()).SetConfigurationProperties (configuration);
            (Cache ?? new CacheSettings ()).SetConfigurationProperties (configuration);
            (Query ?? new QuerySettings ()).SetConfigurationProperties (configuration);
            (Output ?? new OutputSettings ()).SetConfigurationProperties (configuration);
            (Mapping ?? new MappingSettings ()).SetConfigurationProperties (configuration);
            (Proxy ?? new ProxySettings ()).SetConfigurationProperties (configuration);
            (Transaction ?? new TransactionSettings ()).SetConfigurationProperties (configuration);
            (DatabaseSpecific ?? new DatabaseSpecificSettings ()).SetConfigurationProperties (configuration);
        }

        #endregion
    }

    public sealed class CommonSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "dialect".
        /// <para>
        /// The class name of a
        /// NHibernate <see cref="global::NHibernate.Dialect.Dialect" />.
        /// Enables certain platform dependent features. Default value is
        /// <see cref="global::NHibernate.Dialect.SQLiteDialect" />.
        /// </para>
        /// </summary>
        public string Dialect { get; set; } = typeof (global::NHibernate.Dialect.SQLiteDialect).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "default_catalog".
        /// <para>
        /// Qualify unqualified table names with the given catalog name in
        /// generated SQL.
        /// </para>
        /// </summary>
        public string DefaultCatalog { get; set; }

        /// <summary>
        /// Gets or sets the property for "default_schema".
        /// <para>
        /// Qualify unqualified table names with the given schema/table-space
        /// in generated SQL.
        /// </para>
        /// </summary>
        public string DefaultSchema { get; set; }

        /// <summary>
        /// Gets or sets the property for "linqtohql.generatorsregistry".
        /// <para>
        /// The class name of a custom
        /// <see cref="globa::NHibernate.Linq.Functions.ILinqToHqlGeneratorsRegistry" />
        /// implementation. Default value is the built-in
        /// <see cref="globa::NHibernate.Linq.Functions.DefaultLinqToHqlGeneratorsRegistry" />.
        /// </para>
        /// </summary>
        public string LinqToHqlGenerator { get; set; } = typeof (global::NHibernate.Linq.Functions.DefaultLinqToHqlGeneratorsRegistry).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "sql_exception_converter".
        /// <para>
        /// The class name of a custom
        /// <see cref="global::NHibernate.Exceptions.ISQLExceptionConverter" />
        /// implementation. Default value comes from
        /// Dialect.BuildSQLExceptionConverter()
        /// </para>
        /// </summary>
        public string SQLExceptionConverter { get; set; }

        /// <summary>
        /// Gets or sets the property for "collectiontype.factory_class".
        /// <para>
        /// The class name of a custom
        /// <see cref="global::NHibernate.Bytecode.ICollectionTypeFactory" />
        /// implementation. Default value is the built-in
        /// <see cref="global::NHibernate.Type.DefaultCollectionTypeFactory" />.
        /// </para>
        /// </summary>
        public string CollectionFactory { get; set; } = typeof (global::NHibernate.Type.DefaultCollectionTypeFactory).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "default_flush_mode".
        /// <para>
        /// The default <see cref="global::NHibernate.FlushMode" />.
        /// Default value is <see cref="global::NHibernate.FlushMode.Auto" />.
        /// </para>
        /// </summary>
        public global::NHibernate.FlushMode DefaultFlushMode { get; set; } = global::NHibernate.FlushMode.Auto;

        /// <summary>
        /// Gets or sets the property for "default_batch_fetch_size
        /// <para>
        /// The default batch fetch size to use when lazily loading an entity
        /// or collection. Default value is 1.
        /// </para>
        /// </summary>
        public int DefaultBatchFetchSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets the property for "current_session_context_class".
        /// <para>
        /// The class name of an
        /// <see cref="global::NHibernate.Context.ICurrentSessionContext" />
        /// implementation.
        /// </para>
        /// </summary>
        public string CurrentSessionContextClass { get; set; }

        /// <summary>
        /// Gets or sets the property for "id.optimizer.pooled.prefer_lo".
        /// <para>
        /// When  using an enhanced id generator and pooled optimizers prefer
        /// interpreting the database value as the lower (lo) boundary. The
        /// default is to interpret it as the high boundary.
        /// Default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool IdOptimizerPooledPreferLo { get; set; } = false;

        /// <summary>
        /// Gets or sets the property for "generate_statistics".
        /// <para>
        /// Enable statistics collection within
        /// <see cref="global::NHibernate.ISessionFactory.Statistics" />
        /// property. Default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool GenerateStatistics { get; set; } = false;

        /// <summary>
        /// Gets or sets the property for "track_session_id".
        /// <para>
        /// Set whether the session id should be tracked in logs or not. When
        /// <c>true</c>, each session will have an unique
        /// <see cref="System.Guid" /> that can be retrieved with 
        /// <see cref="global::NHibernate.Engine.ISessionImplementor.SessionId" />,
        /// otherwise
        /// <see cref="global::NHibernate.Engine.ISessionImplementor.SessionId" />,
        /// will be <see cref="System.Guid.Empty" />.
        /// </para>
        /// <para>
        /// Session id is used for logging purpose and can also be retrieved on
        /// the static property
        /// <see cref="global::NHibernate.Impl.SessionIdLoggingContext.SessionId" />,
        /// when tracking is enabled.
        /// </para>
        /// <para>
        /// Disabling tracking by setting this property to <c>false</c> increases
        /// performances. Default value is <c>true</c>.
        /// </para>
        /// </summary>
        public bool TrackSessionId { get; set; } = true;

        /// <summary>
        /// Gets or sets the property for "sql_types.keep_datetime".
        /// <para>
        /// Since NHibernate v5.0 and if the dialect supports it,
        /// <see cref="DbType.DateTime2" /> is used instead of
        /// <see cref="DbType.DateTime" />. This may be disabled by setting
        /// this property to <c>true</c>. Default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool KeepDateTime { get; set; } = false;

        #endregion

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.Dialect, Dialect);
            if (!string.IsNullOrWhiteSpace (DefaultCatalog)) {
                configuration.SetProperty (NHEnvironment.DefaultCatalog, DefaultCatalog);
            }
            if (!string.IsNullOrWhiteSpace (DefaultSchema)) {
                configuration.SetProperty (NHEnvironment.DefaultSchema, DefaultSchema);
            }
            configuration.SetProperty (NHEnvironment.LinqToHqlGeneratorsRegistry, LinqToHqlGenerator);
            if (!string.IsNullOrWhiteSpace (SQLExceptionConverter)) {
                configuration.SetProperty (NHEnvironment.SqlExceptionConverter, SQLExceptionConverter);
            }
            configuration.SetProperty (NHEnvironment.CollectionTypeFactoryClass, CollectionFactory);
            configuration.SetProperty (NHEnvironment.DefaultFlushMode, DefaultFlushMode.ToString ());
            configuration.SetProperty (NHEnvironment.DefaultBatchFetchSize, DefaultBatchFetchSize.ToString ());
            if (!string.IsNullOrWhiteSpace (CurrentSessionContextClass)) {
                configuration.SetProperty (NHEnvironment.CurrentSessionContextClass, CurrentSessionContextClass);
            }
            configuration.SetProperty (NHEnvironment.PreferPooledValuesLo, IdOptimizerPooledPreferLo.ToString ());
            configuration.SetProperty (NHEnvironment.GenerateStatistics, GenerateStatistics.ToString ());
            configuration.SetProperty (NHEnvironment.TrackSessionId, TrackSessionId.ToString ());
            configuration.SetProperty (NHEnvironment.SqlTypesKeepDateTime, KeepDateTime.ToString ());
        }

        #endregion
    }

    public sealed class AdoSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "connection.provider".
        /// <para>
        /// Default value is
        /// <see cref="globa::NHibernate.Connection.DriverConnectionProvider" />.
        /// </para>
        /// </summary>
        public string Provider { get; set; } = typeof (global::NHibernate.Connection.DriverConnectionProvider).FullName;

        /// <summary>
        /// Gets or sets the property for "connection.driver_class".
        /// <para>
        /// This is usually not needed, most of the time the dialect will take
        /// care of setting the <see cref="global::NHibernate.Driver.IDriver" />
        /// using a sensible default.
        /// </para>
        /// </summary>
        public string DriverClass { get; set; } = typeof (global::NHibernate.Driver.SQLite20Driver).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for "connection.connection_string".
        /// <para>
        /// Default value is "Data Source=:memory:; Version=3; Page Size=8192;"
        /// </para>
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=:memory:; Version=3; Page Size=8192;";

        /// <summary>
        /// Gets or sets the property for "connection.isolation".
        /// <para>
        /// Set the ADO.NET transaction isolation level. Check
        /// <see cref="IsolationLevel" /> for meaningful values and the
        /// database's documentation to ensure that level is supported. Default
        /// value is <see cref="IsolationLevel.Unspecified" />.
        /// </para>
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Unspecified;

        /// <summary>
        /// Gets or sets the property for "connection.release_mode".
        /// <para>
        /// Specify when NHibernate should release ADO.NET connections. Default
        /// value is <see cref="global::NHibernate.ConnectionReleaseMode.OnClose" />
        /// </para>
        /// </summary>
        public global::NHibernate.ConnectionReleaseMode ConnectionReleaseMode { get; set; } = global::NHibernate.ConnectionReleaseMode.OnClose;

        /// <summary>
        /// Gets or sets the property for "prepare_sql".
        /// <para>
        /// Specify to prepare DbCommands generated by NHibernate. Default
        /// value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool Prepare { get; set; }

        /// <summary>
        /// Gets or sets the property for "command_timeout".
        /// <para>
        /// Specify the default timeout in seconds of DbCommands generated by
        /// NHibernate. Negative values disable it. Default value is 15
        /// seconds.
        /// </para>
        /// </summary>
        public int CommandTimeout { get; set; } = 15;

        /// <summary>
        /// Gets or sets the property for "ado.batch_size".
        /// <para>
        /// Specify the batch size to use when batching update statements.
        /// Setting this to 0 (the default) disables the functionality.
        /// Default value is 16 seconds.
        /// </para>
        /// </summary>
        public int BatchSize { get; set; } = 16;

        /// <summary>
        /// Gets or sets the property for "order_inserts".
        /// Enable ordering of insert statements for the purpose of more
        /// efficient batching. Defaults to <c>true</c> if batching is
        /// enabled, <c>false</c> otherwise.
        /// </para>
        /// </summary>
        public bool? OrderInsert { get; set; }

        /// <summary>
        /// Gets or sets the property for "order_updates".
        /// <para>
        /// Enable ordering of update statements for the purpose of more
        /// efficient batching. Defaults to <c>true</c> if batching is
        /// enabled, <c>false</c> otherwise.
        /// </para>
        /// </summary>
        public bool? OrderUpdate { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.batch_versioned_data".
        /// <para>
        /// If batching is enabled, specify that versioned data can also be
        /// batched. Requires a dialect which batcher correctly returns rows
        /// count. Defaults to <c>false</c>.
        /// </para>
        /// </summary>
        public bool BatchVersionedData { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.factory_class".
        /// <para>
        /// This is usually not needed, most of the time the driver will take
        /// care of setting the <see cref="NHibernate.AdoNet.IBatcherFactory" />
        /// using a sensible default according to the database capabilities.
        /// </para>
        /// </summary>
        public string BatchFactory { get; set; }

        /// <summary>
        /// Gets or sets the property for "adonet.wrap_result_sets".
        /// <para>
        /// Some database vendor data reader implementation have inefficient
        /// columnName-to-columnIndex resolution. Enabling this setting allows
        /// to wrap them in a data reader that will cache those resolutions. 
        /// Defaults to <c>false</c>.
        /// </para>
        /// </summary>
        public bool WrapResultSet { get; set; }

        #endregion

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.ConnectionProvider, Provider);
            configuration.SetProperty (NHEnvironment.ConnectionDriver, DriverClass);
            configuration.SetProperty (NHEnvironment.ConnectionString, ConnectionString);
            configuration.SetProperty (NHEnvironment.Isolation, IsolationLevel.ToString ());
            var connectionReleaseMode = ConnectionReleaseMode
            switch {
                global::NHibernate.ConnectionReleaseMode.AfterTransaction => "after_transaction",
                global::NHibernate.ConnectionReleaseMode.OnClose => "on_close",
                _ => "auto",
            };
            configuration.SetProperty (NHEnvironment.ReleaseConnections, connectionReleaseMode);
            configuration.SetProperty (NHEnvironment.PrepareSql, Prepare.ToString ());
            configuration.SetProperty (NHEnvironment.CommandTimeout, CommandTimeout.ToString ());
            configuration.SetProperty (NHEnvironment.BatchSize, BatchSize.ToString ());
            if (OrderInsert.HasValue) {
                configuration.SetProperty (NHEnvironment.OrderInserts, OrderInsert.Value.ToString ());
            }
            if (OrderUpdate.HasValue) {
                configuration.SetProperty (NHEnvironment.OrderUpdates, OrderUpdate.Value.ToString ());
            }
            configuration.SetProperty (NHEnvironment.BatchVersionedData, BatchVersionedData.ToString ());
            configuration.SetProperty (NHEnvironment.BatchStrategy, BatchFactory);
            configuration.SetProperty (NHEnvironment.WrapResultSets, WrapResultSet.ToString ());
        }

        #endregion
    }

    public sealed class CacheSettings {
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

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.UseSecondLevelCache, UseSecondLevelCache.ToString ());
            configuration.SetProperty (NHEnvironment.CacheProvider, SecondLevelCacheProvider);
            configuration.SetProperty (NHEnvironment.UseMinimalPuts, UseMinimalPuts.ToString ());
            configuration.SetProperty (NHEnvironment.UseQueryCache, UseQueryCache.ToString ());
            configuration.SetProperty (NHEnvironment.QueryCacheFactory, QueryCacheFactory);
            if (!string.IsNullOrWhiteSpace (RegionPrefix)) {
                configuration.SetProperty (NHEnvironment.CacheRegionPrefix, RegionPrefix);
            }
            if (DefaultExpiration.HasValue) {
                configuration.SetProperty (NHEnvironment.UseSecondLevelCache, DefaultExpiration.Value.ToString ());
            }
        }

        #endregion
    }

    public sealed class QuerySettings {
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

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.MaxFetchDepth, MaxFetchDepth.ToString ());
            configuration.SetProperty (NHEnvironment.QuerySubstitutions, Substitutions);
            configuration.SetProperty (NHEnvironment.QueryDefaultCastLength, DefaultCastLength.ToString ());
            configuration.SetProperty (NHEnvironment.QueryDefaultCastPrecision, DefaultCastPrecision.ToString ());
            configuration.SetProperty (NHEnvironment.QueryDefaultCastScale, DefaultCastScale.ToString ());
            configuration.SetProperty (NHEnvironment.QueryStartupChecking, StartUpCheck.ToString ());
            configuration.SetProperty (NHEnvironment.QueryTranslator, Factory);
            configuration.SetProperty (NHEnvironment.QueryLinqProvider, LinqProvider);
            configuration.SetProperty (NHEnvironment.QueryModelRewriterFactory, QueryModelRewriterFactory);
        }

        #endregion
    }

    public sealed class OutputSettings {
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

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.ShowSql, ShowSQL.ToString ());
            configuration.SetProperty (NHEnvironment.FormatSql, FormatSQL.ToString ());
            configuration.SetProperty (NHEnvironment.UseSqlComments, UseSQLComments.ToString ());
        }

        #endregion
    }

    public sealed class MappingSettings {
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

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.Hbm2ddlAuto, Hbm2DllAuto);
            configuration.SetProperty (NHEnvironment.Hbm2ddlKeyWords, Hbm2DllKeywords);
        }

        #endregion
    }

    public sealed class ProxySettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "use_proxy_validator". Enables or
        /// disables validation of interfaces or classes specified as proxies.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool UseProxyValidator { get; set; } = true;

        /// <summary>
        /// Gets or sets the property for "proxyfactory.factory_class". The
        /// class name of a custom
        /// <see cref="global::NHibernate.Bytecode.IProxyFactoryFactory" />
        /// implementation. Default value is the built-in
        /// <see cref="global::NHibernate.Bytecode.StaticProxyFactoryFactory" />.
        /// </summary>
        public string FactoryClass { get; set; } = typeof (global::NHibernate.Bytecode.StaticProxyFactoryFactory).GetQualifiedName ();

        #endregion

        #region Public Properties

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.UseProxyValidator, UseProxyValidator.ToString ());
            configuration.SetProperty (NHEnvironment.ProxyFactoryFactoryClass, FactoryClass);
        }

        #endregion
    }

    public sealed class TransactionSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "transaction.factory_class".
        /// <para>
        /// The class name of a custom
        /// <see cref="global::NHibernate.Transaction.ITransactionFactory" />
        /// implementation. Default value is the built-in
        /// <see cref="global::NHibernate.Transaction.AdoNetWithSystemTransactionFactory" />.
        /// </para>
        /// </summary>
        public string FactoryClass { get; set; } = typeof (global::NHibernate.Transaction.AdoNetWithSystemTransactionFactory).GetQualifiedName ();

        /// <summary>
        /// Gets or sets the property for
        /// "transaction.use_connection_on_system_prepare".
        /// <para>
        /// When a system transaction is being prepared, is using connection
        /// during this process enabled? Default value is <c>true</c>, for
        /// supporting <see cref="global::NHibernate.FlushMode.Commit" /> with
        /// transaction factories supporting system transactions. But this
        /// requires enlisting additional connections, retaining disposed
        /// sessions and their connections until transaction end, and may
        /// trigger undesired transaction promotions to distributed. Set to
        /// <c>false</c> for disabling using connections from system
        /// transaction preparation, while still benefiting from
        /// <see cref="global::NHibernate.FlushMode.Auto" /> on querying.
        /// </para>
        /// </summary>
        public bool? UseConnectionOnSystemPrepare { get; set; }

        /// <summary>
        /// Gets or sets the property for
        /// "transaction.system_completion_lock_timeout".
        /// <para>
        /// Timeout duration in milliseconds for the system transaction
        /// completion lock. When a system transaction completes, it may have
        /// its completion events running on concurrent threads, after scope
        /// disposal. This occurs when the transaction is distributed. This
        /// notably concerns
        /// ISessionImplementor.AfterTransactionCompletion(bool, ITransaction).
        /// NHibernate protects the session from being concurrently used by the
        /// code following the scope disposal with a lock. To prevent any
        /// application freeze, this lock has a default timeout of five seconds.
        /// If the application appears to require longer (!) running
        /// transaction completion events, this setting allows to raise this
        /// timeout. -1 disables the timeout.
        /// </para>
        /// </summary>
        public int SystemCompletionLockTimeout { get; set; } = 5000;

        #endregion

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            configuration.SetProperty (NHEnvironment.TransactionStrategy, FactoryClass);
            if (UseConnectionOnSystemPrepare.HasValue) {
                configuration.SetProperty (NHEnvironment.UseConnectionOnSystemTransactionPrepare, UseConnectionOnSystemPrepare.Value.ToString ());
            }
            configuration.SetProperty (NHEnvironment.SystemTransactionCompletionLockTimeout, SystemCompletionLockTimeout.ToString ());
        }

        #endregion
    }

    public sealed class DatabaseSpecificSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the property for "firebird.disable_parameter_casting".
        /// <para>
        /// Firebird with FirebirdSql.Data.FirebirdClient may be unable to
        /// determine the type of parameters in many circumstances, unless
        /// they are explicitly casted in the SQL query. To avoid this trouble,
        /// the NHibernate FirebirdClientDriver parses SQL commands for
        /// detecting parameters in them and adding an explicit SQL cast around
        /// parameters which may trigger the issue. Default value is
        /// <c>false</c>
        /// </para>
        /// </summary>
        public bool? FirebirdDisableParameterCasting { get; set; } = false;

        /// <summary>
        /// Gets or sets the property for "oracle.use_n_prefixed_types_for_unicode".
        /// <para>
        /// Oracle has a dual Unicode support model.
        /// </para>
        /// <para>
        /// Either the whole database use an Unicode encoding, and then all
        /// string types will be Unicode. In such case, Unicode strings should
        /// be mapped to non <c>N</c> prefixed types, such as <c>Varchar2</c>.
        /// This is the default.
        /// </para>
        /// <para>
        /// Or <c>N</c> prefixed types such as <c>NVarchar2</c> are to be used
        /// for Unicode strings, the others type are using a non Unicode
        /// encoding. In such case this setting needs to be set to <c>true</c>.
        /// </para>
        /// <para>
        /// See https://docs.oracle.com/cd/B19306_01/server.102/b14225/ch6unicode.htm#CACHCAHF.
        /// This setting applies only to Oracle dialects and ODP.Net managed or
        /// unmanaged driver.
        /// </para>
        /// </summary>
        public bool? OracleUseNPrefixedTypesForUnicode { get; set; }

        /// <summary>
        /// Gets or sets the property for "odbc.explicit_datetime_scale".
        /// <para>
        /// This may need to be set to <c>3</c> if you are using the OdbcDriver
        /// with MS SQL Server 2008+.
        /// </para>
        /// <para>
        /// This is intended to work around issues like:
        /// <code>
        /// System.Data.Odbc.OdbcException :
        /// ERROR [22008]
        /// [Microsoft][SQL Server Native Client 11.0]
        /// Datetime field overflow. Fractional second
        /// precision exceeds the scale specified
        /// in the parameter binding.
        /// </code>
        /// </para>
        /// </summary>
        public int? OdbcExplicitDateTimeScale { get; set; }

        #endregion

        #region Public Methods

        public void SetConfigurationProperties (global::NHibernate.Cfg.Configuration configuration) {
            if (FirebirdDisableParameterCasting.HasValue) {
                configuration.SetProperty (NHEnvironment.FirebirdDisableParameterCasting, FirebirdDisableParameterCasting.ToString ());
            }
            if (OracleUseNPrefixedTypesForUnicode.HasValue) {
                configuration.SetProperty (NHEnvironment.OracleUseNPrefixedTypesForUnicode, OracleUseNPrefixedTypesForUnicode.Value.ToString ());
            }
            if (OdbcExplicitDateTimeScale.HasValue) {
                configuration.SetProperty (NHEnvironment.OdbcDateTimeScale, OdbcExplicitDateTimeScale.Value.ToString ());
            }
        }

        #endregion
    }
}