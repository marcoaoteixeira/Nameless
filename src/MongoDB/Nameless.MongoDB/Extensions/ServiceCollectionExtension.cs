using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nameless.MongoDB.Internals;
using Nameless.MongoDB.Options;

namespace Nameless.MongoDB;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers MongoDb configuration.
    /// </summary>
    /// <param name="self">The service collection instance.</param>
    /// <param name="configure">An action to configure MongoDb options.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so other actions can be chained.</returns>
    public static IServiceCollection AddMongoDB(this IServiceCollection self, Action<MongoOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterMongoServices();

    /// <summary>
    /// Registers MongoDb configuration.
    /// </summary>
    /// <param name="self">The service collection instance.</param>
    /// <param name="mongoOptionsConfigSection">The configuration section for Mongo options.</param>
    /// <returns>The current <see cref="IServiceCollection"/> instance so other actions can be chained.</returns>
    public static IServiceCollection AddMongoDB(this IServiceCollection self, IConfigurationSection mongoOptionsConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<MongoOptions>(mongoOptionsConfigSection)
                  .RegisterMongoServices();

    private static IServiceCollection RegisterMongoServices(this IServiceCollection self)
        => self.AddSingleton<IMongoClient>(ResolveMongoClient)
               .AddSingleton(ResolveMongoDatabase)
               .AddSingleton<ICollectionNamingStrategy, CollectionNamingStrategy>()
               .AddSingleton(ResolveMongoCollectionProvider);

    private static MongoClient ResolveMongoClient(IServiceProvider provider) {
        var options = provider.GetOptions<MongoOptions>().Value;

        var settings = new MongoClientSettings {
            Server = new MongoServerAddress(options.Host, options.Port)
        };

        if (TryCreateMongoCredential(options, out var credential)) {
            settings.Credential = credential;
        }

        return new MongoClient(settings);
    }

    private static bool TryCreateMongoCredential(MongoOptions options, [NotNullWhen(returnValue: true)] out MongoCredential? credential) {
        credential = null;

        if (!options.Credentials.UseCredentials) {
            return false;
        }

        var identity = new MongoInternalIdentity(databaseName: options.Credentials.Database,
                                                 username: options.Credentials.Username);
        var evidence = new PasswordEvidence(password: options.Credentials.Password);

        credential = new MongoCredential(mechanism: options.Credentials.Mechanism,
                                         identity: identity,
                                         evidence: evidence);

        return true;
    }

    private static IMongoDatabase ResolveMongoDatabase(IServiceProvider provider) {
        var options = provider.GetOptions<MongoOptions>().Value;

        return provider.GetRequiredService<IMongoClient>()
                       .GetDatabase(options.DatabaseName);
    }

    private static IMongoCollectionProvider ResolveMongoCollectionProvider(IServiceProvider provider) {
        var options = provider.GetOptions<MongoOptions>().Value;
        var logger = provider.GetLogger<MongoOptions>();

        ExecuteDocumentMappers(options.DocumentMappers, logger);

        var database = provider.GetRequiredService<IMongoDatabase>();
        var collectionNamingStrategy = provider.GetRequiredService<ICollectionNamingStrategy>();

        return new MongoCollectionProvider(database, collectionNamingStrategy);
    }

    private static void ExecuteDocumentMappers(string[] documentMappers, ILogger<MongoOptions> logger) {
        foreach (var documentMapper in documentMappers) {
            if (!TryGetDocumentMapperType(documentMapper, logger, out var documentMapperType)) {
                continue;
            }

            if (!TryCreateDocumentMapper(documentMapperType, logger, out var documentMapperInstance)) {
                continue;
            }

            BsonClassMap.RegisterClassMap(documentMapperInstance.CreateMap());
        }
    }

    private static bool TryGetDocumentMapperType(string documentMapper,
                                                 ILogger<MongoOptions> logger,
                                                 [NotNullWhen(returnValue: true)] out Type? documentMapperType) {
        documentMapperType = null;

        try {
            documentMapperType = Type.GetType(documentMapper);

            if (!typeof(DocumentMapperBase).IsAssignableFrom(documentMapperType)) {
                logger.MapperShouldInheritFromDocumentMapperBase(documentMapper);

                documentMapperType = null;
            }
        }
        catch (Exception ex) { logger.ErrorOnLoadingDocumentMapperType(ex, documentMapper); }

        if (documentMapperType is null) {
            logger.DocumentMapperTypeNotLoaded(documentMapper);
        }

        return documentMapperType is not null;
    }

    private static bool TryCreateDocumentMapper(Type documentMapperType,
                                                ILogger<MongoOptions> logger,
                                                [NotNullWhen(returnValue: true)] out DocumentMapperBase? documentMapperInstance) {
        documentMapperInstance = null;

        try { documentMapperInstance = Activator.CreateInstance(documentMapperType) as DocumentMapperBase; }
        catch (Exception ex) { logger.ErrorOnCreatingDocumentMapper(ex, documentMapperType); }

        return documentMapperInstance is not null;
    }
}