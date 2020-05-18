using System.IO;
using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.DependencyInjection.Autofac;
using Nameless.Environment;
using Nameless.Helpers;

namespace Nameless.FileStorage.Physical {
    /// <summary>
    /// The FileStorage service registration.
    /// </summary>
    /// <remarks>
    /// It will use the <see cref="IHostingEnvironment.ApplicationBasePath" />,
    /// combined with "App_Data", as the root path for the
    /// <see cref="IFileStorage" /> implementation.
    /// If <see cref="IHostingEnvironment.ApplicationBasePath" /> is not
    /// present, it will use the <see cref="FileStorageSettings.Root" />
    /// property instead.
    /// </remarks>
    public sealed class FileStorageModule : ModuleBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IFileStorage"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType FileStorageLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

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
            var hostEnvironment = args.Context.ResolveOptional<IHostingEnvironment> ();
            var settings = args.Context.ResolveOptional<FileStorageSettings> () ?? new FileStorageSettings ();

            if (hostEnvironment != null) {
                var path = Path.Combine (hostEnvironment.ApplicationBasePath, "App_Data");
                settings.Root = PathHelper.Normalize (path);
            }

            if (string.IsNullOrWhiteSpace (settings.Root)) {
                settings.Root = typeof (FileStorageModule).Assembly.GetDirectoryPath ();
            }

            args.Parameters = args.Parameters.Union (new Parameter[] {
                TypedParameter.From (settings)
            });
        }

        #endregion
    }
}