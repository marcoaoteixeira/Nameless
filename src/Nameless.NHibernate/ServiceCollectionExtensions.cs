using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.NHibernate.Options;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register NHibernate services.
/// </summary>
public static class ServiceCollectionExtensions {
    public static IServiceCollection ConfigureNHibernateServices(this IServiceCollection self, Action<NHibernateOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                   .AddSingleton(SessionFactoryResolver)
                   .AddScoped(SessionResolver);
    }

    private static ISessionFactory SessionFactoryResolver(this IServiceProvider self) {
        var options = self.GetOptions<NHibernateOptions>();
        var configurationFactory = self.GetRequiredService<IConfigurationFactory>();
        var configuration = configurationFactory.CreateConfiguration();
        var sessionFactory = configuration.BuildSessionFactory();

        if (options.Value.SchemaExport.ExecuteSchemaExport) {
            StartUp(sessionFactory,
                configuration,
                self.GetRequiredService<IApplicationContext>(),
                options.Value.SchemaExport,
                self.GetLogger<SchemaExport>());
        }

        return sessionFactory;
    }

    private static ISession SessionResolver(this IServiceProvider self) {
        return self.GetRequiredService<ISessionFactory>().OpenSession();
    }

    private static void StartUp(ISessionFactory sessionFactory,
                                Configuration configuration,
                                IApplicationContext applicationContext,
                                SchemaExportSettings schemaExportSettings,
                                ILogger<SchemaExport> logger) {
        var outputFilePath = Path.Combine(applicationContext.AppDataFolderPath,
            schemaExportSettings.OutputFolderName,
            $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.dat");

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
                writer);
        }
        catch (Exception ex) { logger.ErrorOnSchemaExportExecution(ex); }
    }
}