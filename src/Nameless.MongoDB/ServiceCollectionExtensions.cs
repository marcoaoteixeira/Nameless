using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nameless.MongoDB.Infrastructure;

namespace Nameless.MongoDB;

public static class ServiceCollectionExtensions {
    private const string MONGO_CLIENT_KEY = $"{nameof(IMongoClient)} :: 4f38fce3-c141-4608-88ad-cf43ce7613e4";
    private const string MONGO_DATABASE_KEY = $"{nameof(IMongoDatabase)} :: b0c8f2d1-3e4a-4b5c-9f6d-7e8f9a0b1c2d";

    private const string COLLECTION_NAMING_STRATEGY_KEY =
        $"{nameof(ICollectionNamingStrategy)} :: 57e05076-3802-42fc-962f-13cd4e8e442b";

    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers MongoDb configuration.
        /// </summary>
        /// <param name="configure">
        ///     An action to configure MongoDb options.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterMongo(Action<MongoOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterMongo();
        }

        /// <summary>
        ///     Registers MongoDb configuration.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterMongo(IConfiguration configuration) {
            var section = configuration.GetSection(nameof(MongoOptions));

            return self.Configure<MongoOptions>(section)
                       .InnerRegisterMongo();
        }

        private IServiceCollection InnerRegisterMongo() {
            // From the documentation: Because each MongoClient represents a pool
            // of connections to the database, most applications require only a
            // single instance of MongoClient
            // See more at https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connect/#std-label-csharp-connect-to-mongodb
            self.TryAddKeyedSingleton(MONGO_CLIENT_KEY, ResolveMongoClient);

            self.TryAddKeyedSingleton(MONGO_DATABASE_KEY, ResolveMongoDatabase);
            self.TryAddKeyedSingleton<ICollectionNamingStrategy>(COLLECTION_NAMING_STRATEGY_KEY, ResolveCollectionNamingStrategy);
            self.TryAddSingleton<IMongoCollectionProvider>(ResolveMongoCollectionProvider);

            return self;
        }
    }

    private static IMongoClient ResolveMongoClient(IServiceProvider provider, object? _) {
        var options = provider.GetRequiredService<IOptions<MongoOptions>>();
        var factory = new MongoClientFactory(options);

        return factory.CreateClient();
    }

    private static IMongoDatabase ResolveMongoDatabase(IServiceProvider provider, object? _) {
        var options = provider.GetRequiredService<IOptions<MongoOptions>>().Value;
        var mongoClient = provider.GetRequiredKeyedService<IMongoClient>(MONGO_CLIENT_KEY);
        var mongoDatabase = mongoClient.GetDatabase(options.DatabaseName);

        foreach (var documentMapping in ResolveDocumentMappings(options)) {
            BsonClassMap.RegisterClassMap(documentMapping.CreateMap());
        }

        return mongoDatabase;
    }

    private static IEnumerable<IDocumentMapping> ResolveDocumentMappings(MongoOptions options) {
        var documentMappings = options.Assemblies
                                      .GetImplementations(typeof(IDocumentMapping))
                                      .Where(type => !type.IsGenericTypeDefinition);

        foreach (var documentMapping in documentMappings) {
            var result = Activator.CreateInstance(documentMapping)
                         ?? throw new InvalidOperationException(
                             $"Couldn't initialize a new instance of the document mapping '{documentMapping.Name}'.");

            yield return (IDocumentMapping)result;
        }
    }

    private static CollectionNamingStrategy ResolveCollectionNamingStrategy(IServiceProvider _, object? __) {
        return new CollectionNamingStrategy();
    }

    private static MongoCollectionProvider ResolveMongoCollectionProvider(IServiceProvider provider) {
        var database = provider.GetRequiredKeyedService<IMongoDatabase>(MONGO_DATABASE_KEY);
        var collectionNamingStrategy = provider.GetRequiredKeyedService<ICollectionNamingStrategy>(COLLECTION_NAMING_STRATEGY_KEY);

        return new MongoCollectionProvider(database, collectionNamingStrategy);
    }
}