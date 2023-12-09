using Autofac;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.NHibernate.Services.Impl;
using NHibernate;
using CoreRoot = Nameless.Root;
using MS_IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using N_IConfigurationBuilder = Nameless.NHibernate.Services.IConfigurationBuilder;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class NHibernateModule : ModuleBase {
        #region Internal Constants

        internal const string CONFIGURATION_BUILDER_TOKEN = $"{nameof(N_IConfigurationBuilder)}::18f15589-f72f-47d9-b0a1-af8391072e3f";
        internal const string SESSION_FACTORY_TOKEN = $"{nameof(ISessionFactory)}::dba2ffdf-3c03-4431-83bc-54746b8e47ab";

        #endregion

        #region Public Constructors

        public NHibernateModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveConfigurationBuilder)
                .Named<N_IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveSessionFactory)
                .Named<ISessionFactory>(SESSION_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveSession)
                .As<ISession>()
                .InstancePerLifetimeScope();

            builder
                .Register(ResolveBootstrapper)
                .As<IStartable>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static NHibernateOptions? GetNHibernateOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<MS_IConfiguration>();
            var options = configuration?
                .GetSection(nameof(NHibernateOptions).RemoveTail(CoreRoot.Defaults.OptsSetsTails))
                .Get<NHibernateOptions>();

            return options;
        }

        private static N_IConfigurationBuilder ResolveConfigurationBuilder(IComponentContext ctx) {
            var options = GetNHibernateOptions(ctx);
            var result = new ConfigurationBuilder(options);

            return result;
        }

        private static ISessionFactory ResolveSessionFactory(IComponentContext ctx) {
            var configurationBuilder = ctx.ResolveNamed<N_IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN);
            var configuration = configurationBuilder.Build();
            var result = configuration.BuildSessionFactory();

            return result;
        }

        private static ISession ResolveSession(IComponentContext ctx) {
            var sessionFactory = ctx.ResolveNamed<ISessionFactory>(SESSION_FACTORY_TOKEN);
            var result = sessionFactory.OpenSession();

            return result;
        }

        private static IStartable ResolveBootstrapper(IComponentContext ctx) {
            var appContext = ctx.ResolveOptional<IApplicationContext>()
                ?? NullApplicationContext.Instance;
            var session = ctx.Resolve<ISession>();
            var configuration = ctx.ResolveNamed<N_IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .Build();
            var options = GetNHibernateOptions(ctx);
            var result = new Bootstrapper(appContext, session, configuration, options?.SchemaExport);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder AddNHibernate(this ContainerBuilder self) {
            self.RegisterModule<NHibernateModule>();

            return self;
        }

        #endregion
    }
}