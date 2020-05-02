using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;

namespace Nameless.FileProvider.Embedded {

    /// <summary>
    /// The FileProvider service registration.
    /// </summary>
    public sealed class FileProviderServiceRegistration : ServiceRegistrationBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IFileProvider"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType FileProviderLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<EmbeddedFileProvider> ()
                .As<IFileProvider> ()
                .SetLifetimeScope (FileProviderLifetimeScope);

            base.Load (builder);
        }

        protected override void AttachToComponentRegistration (Autofac.Core.Registration.IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            var implementationType = registration.Activator.LimitType;

            // verify if the implementation type needs logger injection via constructor.
            var hasConstructorInjection = implementationType.GetConstructors ()
                .Any (constructor => constructor.GetParameters ()
                    .Any (parameter => parameter.ParameterType == typeof (IFileProvider)));

            // if need, inject and return.
            if (hasConstructorInjection) {
                registration.Preparing += (sender, args) => {
                    args.Parameters = args.Parameters.Concat (new [] {
                        TypedParameter.From (implementationType.Assembly)
                    });
                };
                return;
            }
        }

        #endregion
    }
}