using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;
using Nameless.Logging.log4net;
using MS_ILogger = Microsoft.Extensions.Logging.ILogger;
using MS_ILoggerProvider = Microsoft.Extensions.Logging.ILoggerProvider;
using MS_NullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger;

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
                .As<MS_ILoggerProvider>()
                .SingleInstance();
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                // For Nameless
                pipeline.Use(new PropertyResolverMiddleware(
                    serviceType: typeof(ILogger),
                    factory: (member, ctx) => member.DeclaringType != default && member.DeclaringType.FullName != default
                        ? ctx.Resolve<ILoggerFactory>().CreateLogger(member.DeclaringType.FullName)
                        : NullLogger.Instance
                ));

                // For Microsoft
                pipeline.Use(new PropertyResolverMiddleware(
                    serviceType: typeof(MS_ILogger),
                    factory: (member, ctx) => member.DeclaringType != default && member.DeclaringType.FullName != default
                        ? ctx.Resolve<MS_ILoggerProvider>().CreateLogger(member.DeclaringType.FullName)
                        : MS_NullLogger.Instance
                ));
            };
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion
    }
}