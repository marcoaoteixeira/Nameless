using System.Reflection;
using Autofac;
using Autofac.Core;
using MongoDB.Driver;
using Nameless.Autofac;
using Nameless.MongoDB.Impl;
using Nameless.MongoDB.Options;

namespace Nameless.MongoDB.DependencyInjection {
    public sealed class MongoDBModule : ModuleBase {
        #region Private Constants

        private const string MONGO_CLIENT_TOKEN = $"{nameof(IMongoClient)}::4883bd96-fc1e-4fb5-b986-7388163b6be6";
        private const string MONGO_DATABASE_TOKEN = $"{nameof(IMongoDatabase)}::849cd42e-1df2-4097-82b1-bbf88e906808";
        private const string BOOTSTRAPPER_TOKEN = $"{nameof(Bootstrapper)}::6a921d17-56ed-404d-a63d-3bdd9af4b52d";

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
                // Here, our MongoCollectionProvider will be a singleton.
                // So, this will be the perfect place to setup
                // our StartUp code. This must occurs only once.
                .OnActivated(StartUp)
                .SingleInstance();

            var mappings = GetImplementations(typeof(ClassMappingBase<>)).ToArray();
            builder
                .RegisterType<Bootstrapper>()
                .WithParameter(TypedParameter.From(mappings))
                .Named<Bootstrapper>(BOOTSTRAPPER_TOKEN)
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IMongoClient MongoClientResolver(IComponentContext ctx) {
            var options = ctx.GetOptions<MongoOptions>();
            var settings = new MongoClientSettings {
                Server = new(options.Host, options.Port)
            };

            if (options.Credentials.UseCredentials()) {
                var identity = new MongoInternalIdentity(
                    databaseName: options.Credentials.Database,
                    username: options.Credentials.Username
                );
                var evidence = new PasswordEvidence(
                    password: options.Credentials.Password
                );
                var credential = new MongoCredential(
                    mechanism: options.Credentials.Mechanism,
                    identity: identity,
                    evidence: evidence
                );

                settings.Credential = credential;
            }

            var result = new MongoClient(settings);

            return result;
        }

        private static IMongoDatabase MongoDatabaseResolver(IComponentContext ctx) {
            var options = ctx.GetOptions<MongoOptions>();
            var client = ctx.ResolveNamed<IMongoClient>(MONGO_CLIENT_TOKEN);
            var result = client.GetDatabase(options.Database);

            return result;
        }

        private static IMongoCollectionProvider MongoCollectionProviderResolver(IComponentContext ctx) {
            var database = ctx.ResolveNamed<IMongoDatabase>(MONGO_DATABASE_TOKEN);
            var result = new MongoCollectionProvider(
                database,
                CollectionNamingStrategy.Instance
            );

            return result;
        }

        private static void StartUp(IActivatedEventArgs<IMongoCollectionProvider> args)
            => args.Context.ResolveNamed<Bootstrapper>(BOOTSTRAPPER_TOKEN).Run();

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