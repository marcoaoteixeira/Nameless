﻿using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;

namespace Nameless.Logging.log4net {
    /// <summary>
    /// The logging service registration.
    /// </summary>
    public sealed class LoggingModule : ModuleBase {
        #region Private Constants

        private const string LOGGER_EVENT_FACTORY_TOKEN = "LoggerEventFactory.55408158-459f-4bc3-9eaf-2a4e90027dcf";
        private const string LOGGER_FACTORY_TOKEN = "LoggerFactory.6322c41f-c8f0-44d5-8853-2491ea29436b";

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<LoggerEventFactory>()
                .Named<ILoggerEventFactory>(LOGGER_EVENT_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(LoggerFactoryResolver)
                .Named<ILoggerFactory>(LOGGER_FACTORY_TOKEN)
                .SingleInstance();
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(ILogger),
                    factory: (member, ctx) => member.DeclaringType != null && member.DeclaringType.FullName != null
                        ? ctx.ResolveNamed<ILoggerFactory>(LOGGER_FACTORY_TOKEN).CreateLogger(member.DeclaringType.FullName)
                        : NullLogger.Instance
                ));
            };
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static ILoggerFactory LoggerFactoryResolver(IComponentContext context) {
            var loggerEventFactory = context.ResolveNamed<ILoggerEventFactory>(LOGGER_EVENT_FACTORY_TOKEN);
            var options = context.ResolveOptional<Log4netOptions>() ?? Log4netOptions.Default;
            var loggerFactory = new LoggerFactory(loggerEventFactory, options);

            return loggerFactory;
        }

        #endregion
    }
}
