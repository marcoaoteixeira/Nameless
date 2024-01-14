using System.Reflection;
using Autofac;
using MongoDB.Driver;
using Nameless.Autofac;
using Nameless.MongoDB.Impl;

namespace Nameless.MongoDB.DependencyInjection {
    public sealed class MongoDBModule : ModuleBase {
        #region Private Constants

        private const string MONGO_CLIENT_TOKEN = $"{nameof(IMongoClient)}::4883bd96-fc1e-4fb5-b986-7388163b6be6";
        private const string MONGO_DATABASE_TOKEN = $"{nameof(IMongoDatabase)}::849cd42e-1df2-4097-82b1-bbf88e906808";

        #endregion

        #region Public Constructors

        public MongoDBModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(MongoClientResolver)
                .Named<IMongoClient>(MONGO_CLIENT_TOKEN)
                .SingleInstance();

            builder
                .Register(MongoDatabaseResolver)
                .Named<IMongoDatabase>(MONGO_DATABASE_TOKEN)
                .SingleInstance();

            builder
                .Register(MongoCollectionProviderResolver)
                .As<IMongoCollectionProvider>()
                .SingleInstance();

            var mappings = GetImplementations(typeof(ClassMappingBase<>));
            builder
                .RegisterType<Bootstrapper>()
                .WithParameter(TypedParameter.From(mappings))
                .As<IStartable>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IMongoClient MongoClientResolver(IComponentContext ctx) {
            var options = GetOptionsFromContext<MongoOptions>(ctx)
                ?? MongoOptions.Default;
            var settings = new MongoClientSettings {
                Server = new(options.Host, options.Port)
            };
            var result = new MongoClient(settings);

            return result;
        }

        private static IMongoDatabase MongoDatabaseResolver(IComponentContext ctx) {
            var options = GetOptionsFromContext<MongoOptions>(ctx)
                ?? MongoOptions.Default;
            var client = ctx.ResolveNamed<IMongoClient>(MONGO_CLIENT_TOKEN);
            var result = client.GetDatabase(options.Database);

            return result;
        }

        private static IMongoCollectionProvider MongoCollectionProviderResolver(IComponentContext ctx) {
            var database = ctx.ResolveNamed<IMongoDatabase>(MONGO_DATABASE_TOKEN);
            var result = new MongoCollectionProvider(database, CollectionNamingStrategy.Instance);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterMongoDBModule(this ContainerBuilder self, params Assembly[] supportAssemblies) {
            self.RegisterModule(new MongoDBModule(supportAssemblies));

            return self;
        }

        #endregion
    }
}