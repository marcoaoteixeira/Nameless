using Autofac;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.FileProvider {
    public sealed class FileProviderModule : ModuleBase {
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
                    var root = typeof (FileProviderModule).Assembly.GetDirectoryPath ();
                    var environment = ctx.ResolveOptional<IHostEnvironment> ();
                    if (environment != null) { root = environment.ContentRootPath; }
                    var fileProvider = new PhysicalFileProvider (root);
                    return fileProvider;
                })
                .As<IFileProvider> ()
                .SetLifetimeScope (FileProviderLifetimeScope);
        }

        #endregion
    }
}