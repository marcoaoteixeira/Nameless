using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Nameless.Autofac;
using Nameless.MongoDB;
using Nameless.MongoDB.DependencyInjection;
using Nameless.MongoDB.Impl;

namespace Nameless.Data.SQLServer.DependencyInjection {
    public sealed class MongoDBModule : ModuleBase {
        #region Private Constants

        private const string MONGO_CLIENT_TOKEN = $"{nameof(IMongoClient)}::4883bd96-fc1e-4fb5-b986-7388163b6be6";
        private const string MONGO_DATABASE_TOKEN = $"{nameof(IMongoDatabase)}::849cd42e-1df2-4097-82b1-bbf88e906808";

        #endregion

        #region Public Constructors

        public MongoDBModule()
            : base(Array.Empty<Assembly>()) { }

        public MongoDBModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            GetImplementations(typeof(ClassMappingBase<>));

            builder
                .Register(ResolveMongoClient)
                .Named<IMongoClient>(MONGO_CLIENT_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveMongoDatabase)
                .Named<IMongoDatabase>(MONGO_DATABASE_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveMongoCollectionProvider)
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

        private static MongoOptions GetMongoOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(MongoOptions).RemoveTail(new[] { "Options" }))
                .Get<MongoOptions>();

            return options ?? MongoOptions.Default;
        }

        private static IMongoClient ResolveMongoClient(IComponentContext ctx) {
            var options = GetMongoOptions(ctx);
            var settings = new MongoClientSettings {
                Server = new(options.Host, options.Port)
            };
            var result = new MongoClient(settings);

            return result;
        }

        private static IMongoDatabase ResolveMongoDatabase(IComponentContext ctx) {
            var options = GetMongoOptions(ctx);
            var client = ctx.ResolveNamed<IMongoClient>(MONGO_CLIENT_TOKEN);
            var result = client.GetDatabase(options.Database);

            return result;
        }

        private static IMongoCollectionProvider ResolveMongoCollectionProvider(IComponentContext ctx) {
            var database = ctx.ResolveNamed<IMongoDatabase>(MONGO_DATABASE_TOKEN);
            var result = new MongoCollectionProvider(database, DefaultCollectionNamingStrategy.Instance);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddMongoDB(this ContainerBuilder self)
            => self.RegisterModule<MongoDBModule>();

        #endregion
    }
}