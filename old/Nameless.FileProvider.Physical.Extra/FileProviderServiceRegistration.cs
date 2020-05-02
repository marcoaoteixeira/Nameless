using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;
using AF_Parameter = Autofac.Core.Parameter;

namespace Nameless.FileProvider.Physical {

    /// <summary>
    /// The FileProvider service registration.
    /// </summary>
    public sealed class FileProviderServiceRegistration : ServiceRegistrationBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IFileProvider"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType FileProviderLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<PhysicalFileProvider> ()
                .As<IFileProvider> ()
                .OnPreparing (OnPreparingFileProvider)
                .SetLifetimeScope (FileProviderLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Methods

        private void OnPreparingFileProvider (PreparingEventArgs args) {
            var settings = args.Context.ResolveOptional<FileProviderSettings> () ?? new FileProviderSettings ();
            args.Parameters = args.Parameters.Union (new AF_Parameter[] {
                new NamedParameter (
                    name: nameof (FileProviderSettings.Root).ToLower (),
                    value: settings.Root
                )
            });
        }

        #endregion
    }
}