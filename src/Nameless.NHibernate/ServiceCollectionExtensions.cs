using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.NHibernate.Infrastructure;
using Nameless.NHibernate.Internals;
using Nameless.NHibernate.Options;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register NHibernate services.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string SESSION_FACTORY_KEY = $"{nameof(ISessionFactory)} :: 3c7db659-6884-4fb9-92cb-202fa851d967";
    private const string CONFIGURATION_FACTORY_KEY = $"{nameof(IConfigurationFactory)} :: f93b5672-e26c-4d07-9cf3-be1f957a726f";

    /// <summary>
    ///     Registers NHibernate services in the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <param name="configure">
    ///     A configuration action for <see cref="NHibernateOptions"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions can be chained. 
    /// </returns>
    public static IServiceCollection RegisterNHibernate(this IServiceCollection self, Action<NHibernateOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        self.TryAddKeyedSingleton<IConfigurationFactory, ConfigurationFactory>(CONFIGURATION_FACTORY_KEY);
        self.TryAddKeyedSingleton(SESSION_FACTORY_KEY, SessionFactoryResolver);
        self.TryAddTransient(SessionResolver);

        return self;
    }

    private static ISessionFactory SessionFactoryResolver(this IServiceProvider self, object? key) {
        var options = self.GetOptions<NHibernateOptions>();
        var configurationFactory = self.GetRequiredKeyedService<IConfigurationFactory>(CONFIGURATION_FACTORY_KEY);
        var configuration = configurationFactory.CreateConfiguration();
        var sessionFactory = configuration.BuildSessionFactory();

        if (options.Value.SchemaExport.ExecuteSchemaExport) {
            StartUp(
                sessionFactory,
                configuration,
                self.GetRequiredService<IApplicationContext>(),
                options.Value.SchemaExport,
                self.GetLogger<SchemaExport>()
            );
        }

        return sessionFactory;
    }

    private static ISession SessionResolver(this IServiceProvider self) {
        return self.GetRequiredKeyedService<ISessionFactory>(SESSION_FACTORY_KEY).OpenSession();
    }

    private static void StartUp(ISessionFactory sessionFactory,
                                Configuration configuration,
                                IApplicationContext applicationContext,
                                SchemaExportSettings schemaExportSettings,
                                ILogger<SchemaExport> logger) {
        var outputFilePath = Path.Combine(
            applicationContext.DataDirectoryPath,
            schemaExportSettings.OutputDirectoryName,
            $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.dat"
        );

        try {
            using var writer = schemaExportSettings.FileOutput
                ? File.CreateText(outputFilePath)
                : TextWriter.Null;

            using var session = sessionFactory.OpenSession();
            new SchemaExport(configuration).Execute(
                schemaExportSettings.ConsoleOutput,
                schemaExportSettings.Execute,
                schemaExportSettings.JustDrop,
                session.Connection,
                writer
            );
        }
        catch (Exception ex) { logger.ErrorOnSchemaExportExecution(ex); }
    }
}