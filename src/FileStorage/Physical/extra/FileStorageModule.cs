using System.Linq;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Hosting;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.FileStorage.Physical {
    /// <summary>
    /// The FileStorage service registration.
    /// </summary>
    public sealed class FileStorageModule : ModuleBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IFileStorage"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType FileStorageLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<PhysicalFileStorage> ()
                .As<IFileStorage> ()
                .OnPreparing (OnPreparingFileStorage)
                .SetLifetimeScope (FileStorageLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Static Methods

        private static void OnPreparingFileStorage (PreparingEventArgs args) {
            var hostEnvironment = args.Context.ResolveOptional<IHostEnvironment> ();
            var settings = args.Context.ResolveOptional<FileStorageSettings> () ?? new FileStorageSettings ();

            if (hostEnvironment != null) {
                settings.Root = hostEnvironment.ContentRootPath;
            }

            args.Parameters = args.Parameters.Union (new Parameter[] {
                TypedParameter.From (settings)
            });
        }

        #endregion
    }
}