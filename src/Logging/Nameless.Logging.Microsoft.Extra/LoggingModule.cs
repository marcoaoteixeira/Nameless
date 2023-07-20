using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;
using Nameless.Logging.log4net;

namespace Nameless.Logging.Microsoft.Extra {
    public sealed class LoggingModule : ModuleBase {
        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(NullExternalScopeProvider.Instance);

            builder
                .RegisterType<LoggerEventFactory>()
                .As<ILoggerEventFactory>()
                .SingleInstance();

            // Decorators are applied in order of registration.
            builder
                .RegisterDecorator<LoggerEventFactoryDecorator, ILoggerEventFactory>();

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();

            builder
                .RegisterType<LoggerProvider>()
                .As<IMSLoggerProvider>()
                .SingleInstance();
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                // For Nameless
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(ILogger),
                    factory: (member, ctx) => member.DeclaringType != null && member.DeclaringType.FullName != null
                        ? ctx.Resolve<ILoggerFactory>().CreateLogger(member.DeclaringType.FullName)
                        : NullLogger.Instance
                ));

                // For Microsoft
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(IMSLogger),
                    factory: (member, ctx) => member.DeclaringType != null && member.DeclaringType.FullName != null
                        ? ctx.Resolve<IMSLoggerProvider>().CreateLogger(member.DeclaringType.FullName)
                        : MSNullLogger.Instance
                ));
            };
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion
    }
}