using Autofac;
using Autofac.Core;
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
                .RegisterType<HostEnvironment>()
                .As<IHostEnvironment>()
                .OnPreparing(OnPreparingHostEnvironment)
                .SetLifetimeScope(LifetimeScopeType.Singleton);

            base.Load(builder);
        }

        #endregion

        #region Private Methods

        private void OnPreparingHostEnvironment(PreparingEventArgs args) {
            var applicationContext = args.Context.Resolve<IApplicationContext>();

            args.Parameters = args.Parameters.Union(new[] {
                new NamedParameter("environmentName", applicationContext.EnvironmentName),
                new NamedParameter("applicationName", applicationContext.ApplicationName),
                new NamedParameter("applicationBasePath", applicationContext.BasePath)
            });
        }

        #endregion
    }
}
