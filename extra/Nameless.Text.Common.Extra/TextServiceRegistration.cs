using Autofac;
using Nameless.IoC.Autofac;

namespace Nameless.Text {
    /// <summary>
    /// The Text service registration.
    /// </summary>
    public sealed class TextServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IDataBinder"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType DataBinderLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IInterpolator"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType InterpolatorLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<DataBinder> ()
                .As<IDataBinder> ()
                .SetLifetimeScope (DataBinderLifetimeScope);

            builder
                .RegisterType<Interpolator> ()
                .As<IInterpolator> ()
                .SetLifetimeScope (InterpolatorLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}