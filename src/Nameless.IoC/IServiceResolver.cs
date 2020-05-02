using System;

namespace Nameless.IoC {

    /// <summary>
    /// Resolver interface.
    /// </summary>
    public interface IServiceResolver {

        #region Methods

        /// <summary>
        /// Resolves a service by its type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="name">The service name. If any.</param>
        /// <param name="parameters">A collection of parameters.</param>
        /// <returns>The instance of the service.</returns>
        object Get (Type serviceType, string name = null, params Parameter[] parameters);

        #endregion Methods
    }
}