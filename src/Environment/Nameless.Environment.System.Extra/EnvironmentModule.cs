using Autofac;
using Nameless.Autofac;
using Nameless.Infrastructure;

namespace Nameless.Environment.System {
    /// <summary>
    /// The Environment service registration.
    /// </summary>
    public sealed class EnvironmentModule : ModuleBase {
        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(HostEnvironmentResolver)
                .As<IHostEnvironment>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IHostEnvironment HostEnvironmentResolver(IComponentContext context) {
            var applicationContext = context.Resolve<IApplicationContext>();
            var environment = new HostEnvironment(
                environmentName: applicationContext.EnvironmentName,
                applicationName: applicationContext.ApplicationName,
                applicationBasePath: applicationContext.BasePath
            );

            return environment;
        }

        #endregion
    }
}
