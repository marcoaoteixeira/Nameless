using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.NHibernate.Impl;
using Nameless.NHibernate.Options;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

public static class ServiceCollectionExtension {
    #region Private Constants

    private const string SESSION_FACTORY_TOKEN = $"{nameof(ISessionFactory)}::9f122dcc-7d5b-4218-a3da-c8978ed439d9";

    #endregion

    #region Public Static Methods

    public static IServiceCollection RegisterNHibernate(
        this IServiceCollection self,
        Action<NHibernateOptions>? configure = null,
        Type[]? entityTypes = null,
        Type[]? classMappingTypes = null)
        => self
           .AddKeyedSingleton(serviceKey: SESSION_FACTORY_TOKEN,
                              implementationFactory: (provider, _) => GetSessionFactoryImpl(configure, entityTypes, classMappingTypes, provider))
           .AddScoped(provider => provider.GetRequiredKeyedService<ISessionFactory>(SESSION_FACTORY_TOKEN).OpenSession());

    #endregion

    #region Private Static Methods

    private static ISessionFactory GetSessionFactoryImpl(Action<NHibernateOptions>? configure, Type[]? entityTypes, Type[]? classMappingTypes, IServiceProvider provider) {
        var options = provider.GetOptions<NHibernateOptions>();

        configure?.Invoke(options.Value);

        var configurationFactory = new ConfigurationFactory(options.Value,
                                                            entityTypes ?? [],
                                                            classMappingTypes ?? []);

        var sessionFactory = configurationFactory.CreateConfiguration()
                                                 .BuildSessionFactory();

        StartUp(provider, configurationFactory, sessionFactory, options.Value);

        return sessionFactory;
    }

    private static void StartUp(
        IServiceProvider provider,
        IConfigurationFactory configurationFactory,
        ISessionFactory sessionFactory,
        NHibernateOptions options) {
        if (!options.SchemaExport.ExecuteSchemaExport) {
            return;
        }

        var logger = provider.GetLogger(typeof(ServiceCollectionExtension));
        var appContext = provider.GetRequiredService<IApplicationContext>();
        var outputFilePath = Path.Combine(appContext.ApplicationDataFolderPath,
                                          options.SchemaExport.OutputFolderName,
                                          $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.txt");

        try {
            using var writer = options.SchemaExport.FileOutput
                ? File.CreateText(outputFilePath)
                : TextWriter.Null;

            using var session = sessionFactory.OpenSession();
            var configuration = configurationFactory.CreateConfiguration();
            new SchemaExport(configuration).Execute(useStdOut: options.SchemaExport.ConsoleOutput,
                                                    execute: true,
                                                    justDrop: false,
                                                    connection: session.Connection,
                                                    exportOutput: writer);
        } catch (Exception ex) { logger.LogError(exception: ex, message: "Error while initializing NHibernate."); }
    }

    #endregion
}