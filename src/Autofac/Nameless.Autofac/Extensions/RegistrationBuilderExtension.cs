using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;

namespace Nameless.Autofac {

    /// <summary>
    /// <see cref="IRegistrationBuilder{TLimit, TActivatorData, TRegistrationStyle}"/> extension methods.
    /// </summary>
    public static class RegistrationBuilderExtension {

        #region Public Static Methods

        /// <summary>
        /// Specifies that a type from a scanned assembly is registered if it implements an interface
        /// that closes the provided open generic interface type.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TScanningActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to set service mapping on.</param>
        /// <param name="openGenericService">The open generic interface or base class type for which implementations will be found.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static IRegistrationBuilder<TLimit, TScanningActivatorData, TRegistrationStyle> AsClosedInterfacesOf<TLimit, TScanningActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TScanningActivatorData, TRegistrationStyle> registration, Type openGenericService) where TScanningActivatorData : ScanningActivatorData {
            Prevent.Against.Null(openGenericService, nameof(openGenericService));

            if (!openGenericService.IsInterface) {
                throw new ArgumentException("Generic type must be an interface.", nameof(openGenericService));
            }

            return registration
                .Where(candidateType => candidateType.IsClosedTypeOf(openGenericService))
                .As(candidateType => candidateType.GetInterfaces()
                    .Where(_ => _.IsClosedTypeOf(openGenericService))
                    .Select(_ => (Service)new TypedService(_))
                );
        }

        /// <summary>
        /// Sets life time scope type on registration.
        /// </summary>
        /// <typeparam name="TLimit">Type of limit.</typeparam>
        /// <typeparam name="TActivatorData">Type of activator data.</typeparam>
        /// <typeparam name="TRegistrationStyle">Type of registration style.</typeparam>
        /// <param name="self">The registration object.</param>
        /// <param name="lifetimeScopeType">The life time scope type.</param>
        public static void SetLifetimeScope<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> self, LifetimeScopeType lifetimeScopeType) {
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