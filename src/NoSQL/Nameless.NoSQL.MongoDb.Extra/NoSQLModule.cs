using System.Reflection;
using Autofac;
using Autofac.Core;
using MongoDB.Driver;
using Nameless.Autofac;

namespace Nameless.NoSQL.MongoDb {
    public sealed class NoSQLModule : ModuleBase {
        #region Private Constants

        private const string MONGO_CLIENT_KEY = "ed2daa8b-6e05-4c27-9532-11e45b6e2005";
        private const string MONGO_DATABASE_KEY = "2485347f-ad04-42c1-bb3b-b56ddeb34761";
        private const string COLLECTION_NAMING_STRATEGY_KEY = "b88d7123-ea50-4c0e-95cb-28b49d48db0b";
        private const string MONGO_COLLECTION_SETTINGS_KEY = "3c7e71ab-3b08-4330-8b56-a8d1506ed474";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Mongo collection settings.
        /// </summary>
        public MongoCollectionSettings Settings { get; set; } = new();

        /// <summary>
        /// Gets or sets the defined mapping types.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="Array.Empty{Type}"/>
        /// </remarks>
        public Type[] MappingTypes { get; set; } = Array.Empty<Type>();

        #endregion

        #region Public Constructors

        public NoSQLModule(params Assembly[] supportAssemblies) : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            RunMappings(MappingTypes ?? Array.Empty<Type>());

            builder
                .Register(RegisterMongoClient)
                .Named<IMongoClient>(MONGO_CLIENT_KEY)
                .SetLifetimeScope(LifetimeScopeType.Singleton);

            builder
                .Register(RegisterMongoDatabase)
                .Named<IMongoDatabase>(MONGO_DATABASE_KEY)
                .SetLifetimeScope(LifetimeScopeType.Singleton);

            builder
                .RegisterInstance(Settings ?? new())
                .Named<MongoCollectionSettings>(MONGO_COLLECTION_SETTINGS_KEY)
                .SetLifetimeScope(LifetimeScopeType.Singleton);

            builder.Register<ICollectionNamingStrategy, CollectionNamingStrategy>(
                name: COLLECTION_NAMING_STRATEGY_KEY,
                lifetimeScope: LifetimeScopeType.Singleton
            );

            builder
                .Register<IMongoContext, MongoContext>(
                    lifetimeScope: LifetimeScopeType.Singleton,
                    parameters: new[] {
                        ResolvedParameter.ForNamed<IMongoDatabase>(MONGO_DATABASE_KEY),
                        ResolvedParameter.ForNamed<ICollectionNamingStrategy>(COLLECTION_NAMING_STRATEGY_KEY),
                        ResolvedParameter.ForNamed<MongoCollectionSettings>(MONGO_COLLECTION_SETTINGS_KEY)
                    });

            base.Load(builder);
        }

        #endregion

        #region Private Methods

        private static void RunMappings(Type[] mappingTypes) {
            var mappings = mappingTypes.Where(_ => typeof(ClassMappingBase<>).IsAssignableFromGenericType(_));
            foreach (var item in mappings) {
                Activator.CreateInstance(item);
            }
        }

        private static IMongoClient RegisterMongoClient(IComponentContext ctx) {
            var opts = ctx.ResolveOptional<MongoOptions>() ?? MongoOptions.Default;

            var settings = string.IsNullOrWhiteSpace(opts.ConnectionString)
                ? new MongoClientSettings { Server = new MongoServerAddress(opts.Host, opts.Port) }
                : MongoClientSettings.FromConnectionString(opts.ConnectionString);

            if (!string.IsNullOrWhiteSpace(opts.Username) && !string.IsNullOrWhiteSpace(opts.Password)) {
                settings.Credential = MongoCredential.CreateCredential(opts.DatabaseName, opts.Username, opts.Password);
            }

            return new MongoClient(settings);
        }

        private static IMongoDatabase RegisterMongoDatabase(IComponentContext ctx) {
            var opts = ctx.ResolveOptional<MongoOptions>() ?? MongoOptions.Default;
            return ctx.ResolveNamed<IMongoClient>(MONGO_CLIENT_KEY).GetDatabase(opts.DatabaseName);
        }

        #endregion
    }
}
