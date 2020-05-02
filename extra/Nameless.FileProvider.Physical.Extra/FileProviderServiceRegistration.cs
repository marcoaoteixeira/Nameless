using Autofac;
using Microsoft.Extensions.FileProviders;
using Nameless.FileProvider.Common;
using Nameless.IoC.Autofac;

namespace Nameless.FileProvider {
    public sealed class FileProviderServiceRegistration : ServiceRegistrationBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IFileProvider"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType FileProviderLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            builder
                .Register (ctx => {
                    var settings = ctx.ResolveOptional<FileProviderSettings> () ?? new FileProviderSettings ();
                    var fileProvider = new PhysicalFileProvider (settings.Root);
                    return fileProvider;
                })
                .As<IFileProvider> ()
                .SetLifetimeScope (FileProviderLifetimeScope);
        }

        #endregion
    }
}