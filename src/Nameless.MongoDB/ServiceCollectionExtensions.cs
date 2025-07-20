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
    private const string COLLECTION_NAMING_STRATEGY_KEY = $"{nameof(ICollectionNamingStrategy)} :: 57e05076-3802-42fc-962f-13cd4e8e442b";

    /// <summary>
    ///     Registers MongoDb configuration.
    /// </summary>
    /// <param name="self">The service collection instance.</param>
    /// <param name="configure">An action to configure MongoDb options.</param>
    /// <returns>The current <see cref="IServiceCollection" /> instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterMongo(this IServiceCollection self, Action<MongoOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        // From the documentation: Because each MongoClient represents a pool
        // of connections to the database, most applications require only a
        // single instance of MongoClient
        // See more at https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connect/#std-label-csharp-connect-to-mongodb
        self.TryAddKeyedSingleton(MONGO_CLIENT_KEY, ResolveMongoClient);

        self.TryAddKeyedSingleton(MONGO_DATABASE_KEY, ResolveMongoDatabase);
        self.TryAddKeyedSingleton(COLLECTION_NAMING_STRATEGY_KEY, ResolveCollectionNamingStrategy);
        self.TryAddSingleton(ResolveMongoCollectionProvider);

        return self;
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

        foreach (var documentMapper in ResolveDocumentMappers(options)) {
            BsonClassMap.RegisterClassMap(documentMapper.CreateMap());
        }

        return mongoDatabase;
    }

    private static IEnumerable<IDocumentMapper> ResolveDocumentMappers(MongoOptions options) {
        var documentMappers = options.Assemblies
                                     .GetImplementations(typeof(IDocumentMapper))
                                     .Where(type => !type.IsGenericTypeDefinition);

        foreach (var documentMapper in documentMappers) {
            var result = Activator.CreateInstance(documentMapper)
                      ?? throw new InvalidOperationException($"Couldn't initialize a new instance of the document mapper '{documentMapper.Name}'.");

            yield return (IDocumentMapper)result;
        }
    }

    private static ICollectionNamingStrategy ResolveCollectionNamingStrategy(IServiceProvider provider, object? _) {
        return new CollectionNamingStrategy();
    }

    private static IMongoCollectionProvider ResolveMongoCollectionProvider(IServiceProvider provider) {
        var database = provider.GetRequiredKeyedService<IMongoDatabase>(MONGO_DATABASE_KEY);
        var collectionNamingStrategy = provider.GetRequiredKeyedService<ICollectionNamingStrategy>(COLLECTION_NAMING_STRATEGY_KEY);

        return new MongoCollectionProvider(database, collectionNamingStrategy);
    }
}