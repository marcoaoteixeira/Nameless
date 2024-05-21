using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nameless.Autofac;
using Nameless.MongoDB.Impl;
using Nameless.MongoDB.Options;

namespace Nameless.MongoDB.DependencyInjection {
    public sealed class MongoDBModule : ModuleBase {
        #region Private Constants

        private const string MONGO_CLIENT_TOKEN = $"{nameof(MongoClient)}::4883bd96-fc1e-4fb5-b986-7388163b6be6";
        private const string MONGO_DATABASE_TOKEN = $"{nameof(IMongoDatabase)}::849cd42e-1df2-4097-82b1-bbf88e906808";

        #endregion

        #region Public Constructors

        public MongoDBModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            // Why Mongo client is a singleton here?
            // See: https://www.mongodb.com/docs/drivers/csharp/current/faq/#how-does-connection-pooling-work-in-the-.net-c--driver-:~:text=Create%20a%20client%20once%20for%20each%20process%2C%20and%20reuse%20it%20for%20all%20operations.%20It%20is%20a%20common%20mistake%20to%20create%20a%20new%20client%20for%20each%20request%2C%20which%20is%20very%20inefficient
            builder
                .Register(MongoClientResolver)
                .Named<IMongoClient>(MONGO_CLIENT_TOKEN)
                .SingleInstance();

            builder
                .Register(MongoDatabaseResolver)
                .Named<IMongoDatabase>(MONGO_DATABASE_TOKEN)
                .SingleInstance();

            var mappingTypes = GetImplementations(typeof(ClassMappingBase<>)).ToArray();
            builder
                .Register(MongoCollectionProviderResolver)
                .As<IMongoCollectionProvider>()
                // Here, our MongoCollectionProvider will be a singleton.
                // So, this will be the perfect place to setup
                // our StartUp code. This must occurs only once.
                .OnActivated(args => StartUp(args, mappingTypes))
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IMongoClient MongoClientResolver(IComponentContext ctx) {
            var options = ctx.GetPocoOptions<MongoOptions>();
            var settings = new MongoClientSettings {
                Server = new MongoServerAddress(options.Host, options.Port)
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
            var options = ctx.GetPocoOptions<MongoOptions>();
            var client = ctx.ResolveNamed<IMongoClient>(MONGO_CLIENT_TOKEN);
            var result = client.GetDatabase(options.Database);

            return result;
        }

        private static IMongoCollectionProvider MongoCollectionProviderResolver(IComponentContext ctx) {
            var database = ctx.ResolveNamed<IMongoDatabase>(MONGO_DATABASE_TOKEN);
            var result = new MongoCollectionProvider(database,
                                                     CollectionNamingStrategy.Instance);

            return result;
        }

        private static void StartUp(IActivatedEventArgs<IMongoCollectionProvider> args, Type[] mappingTypes) {
            var logger = args.Context.GetLogger<MongoDBModule>();

            foreach (var mappingType in mappingTypes) {
                try {
                    var argumentType = GetArgumentType(mappingType);
                    var classMapMethod = GetClassMapMethod(mappingType, argumentType);
                    var bsonClassMap = CreateBsonClassMap(argumentType);
                    var classMapping = Activator.CreateInstance(mappingType);

                    classMapMethod
                        .Invoke(obj: classMapping,
                                parameters: [bsonClassMap]);

                } catch (Exception ex) {
                    logger.LogError(exception: ex,
                                    message: "Error while running mapping {MappingType}",
                                    args: mappingType.FullName);
                }
            }
        }

        private static Type GetArgumentType(Type? mappingType) {
            while (true) {
                if (mappingType is null) {
                    throw new InvalidOperationException("Argument type not found.");
                }

                var argumentType = mappingType.GetGenericArguments()
                                              .FirstOrDefault();

                if (argumentType is not null) {
                    return argumentType;
                }

                mappingType = mappingType.BaseType;
            }
        }

        private static MethodInfo GetClassMapMethod(Type mappingType, Type argumentType) {
            var method = mappingType.GetMethod(name: nameof(ClassMappingBase<object>.Map))
                ?? throw new InvalidOperationException($"Method {nameof(ClassMappingBase<object>.Map)}<{argumentType.Name}>() not found.");

            return method;
        }

        private static object CreateBsonClassMap(Type argumentType) {
            var method = typeof(BsonClassMap)
                .GetMethods()
                .FirstOrDefault(method => method.IsGenericMethod &&
                                          method.Name == nameof(BsonClassMap.RegisterClassMap))
                ?? throw new InvalidOperationException($"Method {nameof(BsonClassMap.RegisterClassMap)}<{argumentType.Name}>() not found.");

            var result = method
                .MakeGenericMethod(typeArguments: [argumentType])
                .Invoke(obj: null,
                        parameters: null);

            return result ?? throw new InvalidOperationException($"Impossible to execute {nameof(BsonClassMap)}.{nameof(BsonClassMap.RegisterClassMap)}<{argumentType.Name}>()");
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