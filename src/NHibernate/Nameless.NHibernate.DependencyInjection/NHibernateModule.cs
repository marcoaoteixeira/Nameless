using Autofac;
using Autofac.Core;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.NHibernate.Impl;
using Nameless.NHibernate.Options;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class NHibernateModule : ModuleBase {
        #region Internal Constants

        internal const string CONFIGURATION_FACTORY_TOKEN = $"{nameof(IConfigurationFactory)}::18f15589-f72f-47d9-b0a1-af8391072e3f";
        internal const string SESSION_FACTORY_TOKEN = $"{nameof(ISessionFactory)}::dba2ffdf-3c03-4431-83bc-54746b8e47ab";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ConfigurationFactoryResolver)
                .Named<IConfigurationFactory>(CONFIGURATION_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(SessionFactoryResolver)
                .Named<ISessionFactory>(SESSION_FACTORY_TOKEN)
                // Here, our SessionFactory will be a singleton.
                // So, this will be the perfect place to setup
                // our StartUp code. This must occurs only once.
                .OnActivated(StartUp)
                .SingleInstance();

            builder
                .Register(SessionResolver)
                .As<ISession>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConfigurationFactory ConfigurationFactoryResolver(IComponentContext ctx) {
            var options = ctx.GetPocoOptions<NHibernateOptions>();
            var result = new ConfigurationFactory(options);

            return result;
        }

        private static ISessionFactory SessionFactoryResolver(IComponentContext ctx) {
            var configuration = ctx
                .ResolveNamed<IConfigurationFactory>(CONFIGURATION_FACTORY_TOKEN)
                .CreateConfiguration();
            var result = configuration.BuildSessionFactory();

            return result;
        }

        private static ISession SessionResolver(IComponentContext ctx) {
            var sessionFactory = ctx.ResolveNamed<ISessionFactory>(SESSION_FACTORY_TOKEN);
            var result = sessionFactory.OpenSession();

            return result;
        }

        private static void StartUp(IActivatedEventArgs<ISessionFactory> args) {
            var ctx = args.Context;
            var options = ctx.GetPocoOptions<NHibernateOptions>();

            if (!options.SchemaExport.ExecuteSchemaExport) {
                return;
            }

            var appContext = ctx.Resolve<IApplicationContext>();
            var outputFilePath = Path.Combine(appContext.ApplicationDataFolderPath,
                                              options.SchemaExport.OutputFolderName,
                                              $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.txt");

            using var writer = options.SchemaExport.FileOutput
                ? File.CreateText(outputFilePath)
                : TextWriter.Null;

            using var session = args.Instance.OpenSession();
            var configuration = ctx
                .ResolveNamed<IConfigurationFactory>(CONFIGURATION_FACTORY_TOKEN)
                .CreateConfiguration();
            new SchemaExport(configuration).Execute(useStdOut: options.SchemaExport.ConsoleOutput,
                                                    execute: true,
                                                    justDrop: false,
                                                    connection: session.Connection,
                                                    exportOutput: writer);
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterNHibernateModule(this ContainerBuilder self) {
            self.RegisterModule<NHibernateModule>();

            return self;
        }

        #endregion
    }
}