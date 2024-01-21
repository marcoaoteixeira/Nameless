using Autofac;
using Autofac.Core;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.NHibernate.Options;
using Nameless.NHibernate.Services;
using Nameless.NHibernate.Services.Impl;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class NHibernateModule : ModuleBase {
        #region Internal Constants

        internal const string CONFIGURATION_BUILDER_TOKEN = $"{nameof(IConfigurationBuilder)}::18f15589-f72f-47d9-b0a1-af8391072e3f";
        internal const string SESSION_FACTORY_TOKEN = $"{nameof(ISessionFactory)}::dba2ffdf-3c03-4431-83bc-54746b8e47ab";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ConfigurationBuilderResolver)
                .Named<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .SingleInstance();

            builder
                .Register(SessionFactoryResolver)
                .Named<ISessionFactory>(SESSION_FACTORY_TOKEN)
                .OnActivated(StartUp) // StartUp should only occurs once.
                .SingleInstance();

            builder
                .Register(SessionResolver)
                // Here, our Session will be a singleton.
                // So, this will be the perfect place to setup
                // our StartUp code. This must occurs only once.
                .As<ISession>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConfigurationBuilder ConfigurationBuilderResolver(IComponentContext ctx) {
            var options = ctx.GetOptions<NHibernateOptions>();
            var result = new ConfigurationBuilder(options);

            return result;
        }

        private static ISessionFactory SessionFactoryResolver(IComponentContext ctx) {
            var configuration = ctx
                .ResolveNamed<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .Build();
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
            var options = ctx.GetOptions<NHibernateOptions>();

            if (!options.SchemaExport.ExecuteSchemaExport) {
                return;
            }

            var appContext = ctx.ResolveOptional<IApplicationContext>()
                ?? NullApplicationContext.Instance;
            var outputFilePath = Path.Combine(
                appContext.ApplicationDataFolderPath,
                options.SchemaExport.OutputFolderName,
                $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.txt"
            );

            using var writer = options.SchemaExport.FileOutput
                ? File.CreateText(outputFilePath)
                : TextWriter.Null;

            using var session = args.Instance.OpenSession();
            var configuration = ctx
                .ResolveNamed<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .Build();
            new SchemaExport(configuration).Execute(
                useStdOut: options.SchemaExport.ConsoleOutput,
                execute: true,
                justDrop: false,
                connection: session.Connection,
                exportOutput: writer
            );
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