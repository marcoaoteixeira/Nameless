﻿using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;

namespace Nameless.Autofac {

    /// <summary>
    /// Autofac extension methods
    /// </summary>
    public static class AutofacExtension {

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
            Garda.Prevent.Null(openGenericService, nameof(openGenericService));

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

        #endregion
    }
}