using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Nameless.MongoDB;

public static class ServiceCollectionExtensions {
    private const string MONGO_CLIENT_KEY = $"{nameof(IMongoClient)} :: 4f38fce3-c141-4608-88ad-cf43ce7613e4";
    private const string MONGO_DATABASE_KEY = $"{nameof(IMongoDatabase)} :: b0c8f2d1-3e4a-4b5c-9f6d-7e8f9a0b1c2d";

    /// <summary>
    ///     Registers MongoDb configuration.
    /// </summary>
    /// <param name="self">The service collection instance.</param>
    /// <param name="configure">An action to configure MongoDb options.</param>
    /// <returns>The current <see cref="IServiceCollection" /> instance so other actions can be chained.</returns>
    public static IServiceCollection ConfigureMongoServices(this IServiceCollection self, Action<MongoOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new MongoOptions();

        innerConfigure(options);

        return self.RegisterMainServices(options);
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self, MongoOptions options) {
        return self
              // From the documentation: Because each MongoClient represents a pool
              // of connections to the database, most applications require only a
              // single instance of MongoClient
              // See more at https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connect/#std-label-csharp-connect-to-mongodb
              .AddKeyedSingleton(MONGO_CLIENT_KEY, ResolveMongoClient(options))

              .AddKeyedSingleton(MONGO_DATABASE_KEY, (provider, _) => ResolveMongoDatabase(provider, options))

              .AddSingleton<ICollectionNamingStrategy, CollectionNamingStrategy>()

              .AddSingleton(ResolveMongoCollectionProvider);
    }

    private static IMongoClient ResolveMongoClient(MongoOptions options) {
        var settings = new MongoClientSettings {
            Server = new MongoServerAddress(options.Host, options.Port)
        };

        if (options.Credentials.UseCredentials) {
            var identity = new MongoInternalIdentity(options.Credentials.Database, options.Credentials.Username);
            var evidence = new PasswordEvidence(options.Credentials.Password);
            var credential = new MongoCredential(options.Credentials.Mechanism, identity, evidence);

            settings.Credential = credential;
        }

        return new MongoClient(settings);
    }

    private static IMongoDatabase ResolveMongoDatabase(IServiceProvider provider, MongoOptions options) {
        var mongoClient = provider.GetRequiredKeyedService<IMongoClient>(MONGO_CLIENT_KEY);
        var mongoDatabase = mongoClient.GetDatabase(options.DatabaseName);

        foreach (var documentMapper in ResolveDocumentMappers(options)) {
            BsonClassMap.RegisterClassMap(documentMapper.CreateMap());
        }

        return mongoDatabase;
    }

    private static IEnumerable<IDocumentMapper> ResolveDocumentMappers(MongoOptions options) {
        var documentMappers = options.Assemblies
                                     .GetImplementations([typeof(IDocumentMapper)])
                                     .Where(type => !type.IsGenericTypeDefinition);

        foreach (var documentMapper in documentMappers) {
            var result = Activator.CreateInstance(documentMapper)
                      ?? throw new InvalidOperationException($"Couldn't initialize a new instance of the document mapper '{documentMapper.Name}'.");

            yield return (IDocumentMapper)result;
        }
    }

    private static IMongoCollectionProvider ResolveMongoCollectionProvider(IServiceProvider provider) {
        var database = provider.GetRequiredKeyedService<IMongoDatabase>(MONGO_DATABASE_KEY);
        var namingStrategy = provider.GetRequiredService<ICollectionNamingStrategy>();

        return new MongoCollectionProvider(database, namingStrategy);
    }
}