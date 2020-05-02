using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AF_Module = Autofac.Module;

namespace Nameless.IoC.Autofac {

    /// <summary>
    /// Abstract implementation of <see cref="IServiceRegistration"/> for Autofac (https://autofac.org/).
    /// </summary>
    public abstract class ServiceRegistrationBase : AF_Module, IServiceRegistration {

        #region Protected Properties

        /// <summary>
        /// Gets the support assemblies.
        /// </summary>
        protected IEnumerable<Assembly> SupportAssemblies { get; }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Protected constructor.
        /// </summary>
        protected ServiceRegistrationBase ()
            : this (Enumerable.Empty<Assembly> ()) { }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        protected ServiceRegistrationBase (IEnumerable<Assembly> supportAssemblies) {
            SupportAssemblies = supportAssemblies ?? Array.Empty<Assembly> ();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Searches for an implementation for a given service type.
        /// </summary>
        /// <typeparam name="TType">The service type.</typeparam>
        /// <returns>The service type implementation</returns>
        protected Type SearchForImplementation<TType> () {
            return SearchForImplementation (typeof (TType));
        }

        /// <summary>
        /// Searches for implementations for a given service type.
        /// </summary>
        /// <typeparam name="TType">The service type.</typeparam>
        /// <returns>An array of types</returns>
        protected Type[] SearchForImplementations<TType> () {
            return SearchForImplementations (typeof (TType));
        }

        /// <summary>
        /// Searches for an implementation for a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service type implementation</returns>
        protected Type SearchForImplementation (Type serviceType) {
            return SearchForImplementations (serviceType).SingleOrDefault ();
        }

        /// <summary>
        /// Searches for implementations for a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>An array of types</returns>
        protected Type[] SearchForImplementations (Type serviceType) {
            Prevent.ParameterNull (serviceType, nameof (serviceType));

            if (!SupportAssemblies.Any ()) { return Enumerable.Empty<Type> ().ToArray (); }
            var result = SupportAssemblies
                .SelectMany (assembly => assembly.GetExportedTypes ())
                .Where (type => !type.GetTypeInfo ().IsAbstract && !type.GetTypeInfo ().IsInterface)
                .Where (type => serviceType.IsAssignableFrom (type) || type.IsAssignableToGenericType (serviceType))
                .Where (type => type.GetCustomAttribute<NullObjectAttribute> (inherit: false) == null)
                .ToArray ();
            return result;
        }

        #endregion

        #region IServiceRegistration Members

        /// <inheritdoc/>
        /// <remarks>
        /// You must never call Autofac "ContainerBuilder.Build()" in the <see cref="Register"/> method.
        /// </remarks>
        public void Register () { /* Do nothing */ }

        #endregion
    }
}