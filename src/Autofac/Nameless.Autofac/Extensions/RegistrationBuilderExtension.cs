using Autofac.Builder;

namespace Nameless.Autofac {

    /// <summary>
    /// Extension methods for <see cref="IRegistrationBuilder{TLimit, TActivatorData, TRegistrationStyle}"/>
    /// </summary>
    public static class RegistrationBuilderExtension {

        #region Public Static Methods

        /// <summary>
        /// Sets life time scope type on registration.
        /// </summary>
        /// <typeparam name="TLimit">Type of limit.</typeparam>
        /// <typeparam name="TActivatorData">Type of activator data.</typeparam>
        /// <typeparam name="TRegistrationStyle">Type of registration style.</typeparam>
        /// <param name="self">The registration object.</param>
        /// <param name="lifetimeScopeType">The life time scope type.</param>
        public static void SetLifetimeScope<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> self, LifetimeScopeType lifetimeScopeType) {
            if (self == default) { return; }

            switch (lifetimeScopeType) {
                case LifetimeScopeType.Singleton:
                    self.SingleInstance();
                    break;

                case LifetimeScopeType.Transient:
                    self.InstancePerDependency();
                    break;

                case LifetimeScopeType.PerScope:
                    self.InstancePerLifetimeScope();
                    break;

                default:
                    throw new Exception("Lifetime scope not defined.");
            }
        }

        #endregion
    }
}