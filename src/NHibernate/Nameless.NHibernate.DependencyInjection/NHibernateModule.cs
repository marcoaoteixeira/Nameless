using Autofac;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.NHibernate.Services;
using Nameless.NHibernate.Services.Impl;
using NHibernate;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class NHibernateModule : ModuleBase {
        #region Internal Constants

        internal const string CONFIGURATION_BUILDER_TOKEN = $"{nameof(IConfigurationBuilder)}::18f15589-f72f-47d9-b0a1-af8391072e3f";
        internal const string SESSION_FACTORY_TOKEN = $"{nameof(ISessionFactory)}::dba2ffdf-3c03-4431-83bc-54746b8e47ab";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveConfigurationBuilder)
                .Named<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
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

        private static IConfigurationBuilder ResolveConfigurationBuilder(IComponentContext ctx) {
            var options = GetOptionsFromContext<NHibernateOptions>(ctx)
                ?? NHibernateOptions.Default;
            var result = new ConfigurationBuilder(options);

            return result;
        }

        private static ISessionFactory ResolveSessionFactory(IComponentContext ctx) {
            var configurationBuilder = ctx.ResolveNamed<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN);
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
            var configuration = ctx.ResolveNamed<IConfigurationBuilder>(CONFIGURATION_BUILDER_TOKEN)
                .Build();
            var options = GetOptionsFromContext<NHibernateOptions>(ctx)
                ?? NHibernateOptions.Default;
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