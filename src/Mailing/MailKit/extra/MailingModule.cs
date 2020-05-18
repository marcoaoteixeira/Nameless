using System.IO;
using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.DependencyInjection.Autofac;
using Nameless.Environment;
using Nameless.FileStorage;
using Nameless.Helpers;

namespace Nameless.Mailing.MailKit {
    /// <summary>
    /// The Mailing service registration.
    /// </summary>
    public sealed class MailingModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IMailingService"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType MailingServiceLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<MailingService> ()
                .As<IMailingService> ()
                .OnPreparing (OnPreparingMailingService)
                .SetLifetimeScope (MailingServiceLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Static Methods

        private static void OnPreparingMailingService (PreparingEventArgs args) {
            var fileStorage = args.Context.Resolve<IFileStorage> ();
            var hostEnvironment = args.Context.ResolveOptional<IHostingEnvironment> ();
            var settings = args.Context.ResolveOptional<MailingSettings> () ?? new MailingSettings ();

            if (hostEnvironment != null) {
                var path = Path.Combine (hostEnvironment.ApplicationBasePath, "App_Data", MailingSettings.Default.PickupDirectoryFolder);
                settings.PickupDirectoryFolder = PathHelper.Normalize (path);
            }

            args.Parameters = args.Parameters.Union (new Parameter[] {
                TypedParameter.From (fileStorage),
                TypedParameter.From (settings)
            });
        }

        #endregion
    }
}