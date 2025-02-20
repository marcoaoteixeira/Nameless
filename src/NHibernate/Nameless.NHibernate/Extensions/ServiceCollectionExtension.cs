using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.NHibernate.Options;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddNHibernate(this IServiceCollection self, Action<NHibernateOptions> configure)
        => self.Configure(configure)
               .RegisterNHibernateServices();

    public static IServiceCollection AddNHibernate(this IServiceCollection self, IConfigurationSection nhibernateOptionsConfigurationSection)
        => self.Configure<NHibernateOptions>(nhibernateOptionsConfigurationSection)
               .RegisterNHibernateServices();

    private static IServiceCollection RegisterNHibernateServices(this IServiceCollection self)
        => self.AddSingleton<IConfigurationFactory, ConfigurationFactory>()
               .AddSingleton(SessionFactoryResolver)
               .AddTransient(SessionResolver);

    private static ISessionFactory SessionFactoryResolver(this IServiceProvider self) {
        var options = self.GetOptions<NHibernateOptions>();
        var configurationFactory = self.GetRequiredService<IConfigurationFactory>();
        var configuration = configurationFactory.CreateConfiguration();
        var sessionFactory = configuration.BuildSessionFactory();

        if (options.Value.SchemaExportSettings.ExecuteSchemaExport) {
            StartUp(sessionFactory: sessionFactory,
                    configuration: configuration,
                    applicationContext: self.GetRequiredService<IApplicationContext>(),
                    schemaExportSettings: options.Value.SchemaExportSettings,
                    logger: self.GetLogger<SchemaExport>());
        }

        return sessionFactory;
    }

    private static ISession SessionResolver(this IServiceProvider self)
        => self.GetRequiredService<ISessionFactory>().OpenSession();

    private static void StartUp(ISessionFactory sessionFactory,
                                Configuration configuration,
                                IApplicationContext applicationContext,
                                SchemaExportSettings schemaExportSettings,
                                ILogger<SchemaExport> logger) {
        
        var outputFilePath = Path.Combine(applicationContext.AppDataFolderPath,
                                          schemaExportSettings.OutputFolderName,
                                          $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.txt");

        try {
            using var writer = schemaExportSettings.FileOutput
                ? File.CreateText(outputFilePath)
                : TextWriter.Null;

            using var session = sessionFactory.OpenSession();
            new SchemaExport(configuration).Execute(useStdOut: schemaExportSettings.ConsoleOutput,
                                                    execute: schemaExportSettings.Execute,
                                                    justDrop: schemaExportSettings.JustDrop,
                                                    connection: session.Connection,
                                                    exportOutput: writer);
        } catch (Exception ex) { logger.ErrorOnSchemaExportExecution(ex); }
    }
}