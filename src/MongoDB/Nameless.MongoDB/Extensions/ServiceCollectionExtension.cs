using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nameless.MongoDB.Impl;
using Nameless.MongoDB.Options;

namespace Nameless.MongoDB;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers MongoDb configuration.
    /// </summary>
    /// <param name="self">The service collection instance.</param>
    /// <param name="configure">An action to configure MongoDb options.</param>
    /// <param name="collectionNamingStrategy">The collection naming strategy, if <c>null</c> will use <see cref="CollectionNamingStrategy.Instance"/>.</param>
    /// <param name="supportAssemblies">Support assemblies to look for implementations of <see cref="ClassMappingBase{TDocument}"/></param>
    /// <returns>The current instance of <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterMongoCollectionProvider(
        this IServiceCollection self,
        Action<MongoOptions>? configure = null,
        ICollectionNamingStrategy? collectionNamingStrategy = null,
        params Assembly[] supportAssemblies)
        => Prevent.Argument
                  .Null(self, nameof(self))
                  .AddSingleton<IMongoCollectionProvider>(provider => {
                      var options = provider.GetOptions<MongoOptions>();

                      configure?.Invoke(options.Value);

                      var classMappingTypes = FetchClassMappingTypes(supportAssemblies);
                      StartUp(provider, classMappingTypes);

                      return new MongoCollectionProvider(
                          database: GetMongoDatabase(options.Value),
                          collectionNamingStrategy: collectionNamingStrategy ?? CollectionNamingStrategy.Instance
                      );
                  });

    private static bool TryGetMongoCredential(MongoOptions options, [NotNullWhen(returnValue: true)] out MongoCredential? credential) {
        credential = null;

        if (!options.Credentials.UseCredentials) {
            return false;
        }

        var identity = new MongoInternalIdentity(
            databaseName: options.Credentials.Database,
            username: options.Credentials.Username
        );
        var evidence = new PasswordEvidence(
            password: options.Credentials.Password
        );
                
        credential = new MongoCredential(
            mechanism: options.Credentials.Mechanism,
            identity: identity,
            evidence: evidence
        );

        return true;
    }

    private static IMongoDatabase GetMongoDatabase(MongoOptions options) {
        var settings = new MongoClientSettings {
            Server = new MongoServerAddress(options.Host, options.Port)
        };

        if (TryGetMongoCredential(options, out var credentials)) {
            settings.Credential = credentials;
        }

        return new MongoClient(settings).GetDatabase(options.Database);
    }

    private static void StartUp(IServiceProvider provider, IEnumerable<Type> classMappingTypes) {
        var logger = provider.GetLogger(typeof(ServiceCollectionExtension));

        foreach (var classMappingType in classMappingTypes) {
            if (!classMappingType.HasParameterlessConstructor()) {
                logger.LogInformation(message: "Mapping {MappingType} ignored. Type requires parameterless constructor.",
                                      args: classMappingType.FullName);
                continue;
            }

            try {
                var documentType = GetDocumentType(classMappingType);
                var mapHandler = GetMapHandler(classMappingType, documentType);
                var bsonClassMap = CreateBsonClassMap(documentType);
                var classMappingInstance = Activator.CreateInstance(classMappingType);

                mapHandler.Invoke(obj: classMappingInstance,
                                  parameters: [bsonClassMap]);

            } catch (Exception ex) {
                logger.LogError(exception: ex,
                                message: "Error while running mapping {MappingType}",
                                args: classMappingType.FullName);
            }
        }
    }

    private static Type GetDocumentType(Type? classMappingType) {
        while (true) {
            if (classMappingType is null) {
                throw new InvalidOperationException("Argument type not found.");
            }

            var argumentType = classMappingType.GetGenericArguments()
                                               .FirstOrDefault();

            if (argumentType is not null) {
                return argumentType;
            }

            classMappingType = classMappingType.BaseType;
        }
    }

    private static MethodInfo GetMapHandler(Type classMappingType, MemberInfo documentType) {
        var method = classMappingType.GetMethod(name: nameof(ClassMappingBase<object>.Map))
                  ?? throw new InvalidOperationException($"Method {nameof(ClassMappingBase<object>.Map)}<{documentType.Name}>() not found.");

        return method;
    }

    private static object CreateBsonClassMap(Type documentType) {
        var method = typeof(BsonClassMap).GetMethods()
                                         .FirstOrDefault(method => method is {
                                             IsGenericMethod: true,
                                             Name: nameof(BsonClassMap.RegisterClassMap)
                                         }) ?? throw new InvalidOperationException($"Method {nameof(BsonClassMap.RegisterClassMap)}<{documentType.Name}>() not found.");

        var result = method.MakeGenericMethod(typeArguments: [documentType])
                           .Invoke(obj: null,
                                   parameters: null);

        return result ?? throw new InvalidOperationException($"Impossible to execute {nameof(BsonClassMap)}.{nameof(BsonClassMap.RegisterClassMap)}<{documentType.Name}>()");
    }

    private static IEnumerable<Type> FetchClassMappingTypes(IEnumerable<Assembly> supportAssemblies)
        => supportAssemblies.SelectMany(assembly => assembly.SearchForImplementations(typeof(ClassMappingBase<>)));
}