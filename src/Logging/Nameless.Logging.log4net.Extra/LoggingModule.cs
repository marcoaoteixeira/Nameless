using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;

namespace Nameless.Logging.log4net {

    /// <summary>
    /// The logging service registration.
    /// </summary>
    public sealed class LoggingModule : ModuleBase {

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<LoggerEventFactory>()
                .As<ILoggerEventFactory>()
                .SingleInstance();

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolverMiddleware(
                    serviceType: typeof(ILogger),
                    factory: (member, ctx) => member.DeclaringType != default && member.DeclaringType.FullName != default
                        ? ctx.Resolve<ILoggerFactory>().CreateLogger(member.DeclaringType.FullName)
                        : NullLogger.Instance
                ));
            };
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion
    }
}
