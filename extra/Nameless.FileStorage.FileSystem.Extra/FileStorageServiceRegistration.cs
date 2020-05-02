using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;

namespace Nameless.FileStorage.FileSystem {
    /// <summary>
    /// The FileStorage service registration.
    /// </summary>
    public sealed class FileStorageServiceRegistration : ServiceRegistrationBase {
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
                .RegisterType<FileSystemStorage> ()
                .As<IFileStorage> ()
                .OnPreparing (OnPreparingFileStorage)
                .SetLifetimeScope (FileStorageLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Static Methods

        private static void OnPreparingFileStorage (PreparingEventArgs args) {
            var settings = args.Context.ResolveOptional<FileStorageSettings> () ?? new FileStorageSettings ();
            args.Parameters = args.Parameters.Union (new Parameter[] {
                new NamedParameter (nameof (FileStorageSettings.Root).ToLower(), settings.Root ?? string.Empty)
            });
        }

        #endregion
    }
}