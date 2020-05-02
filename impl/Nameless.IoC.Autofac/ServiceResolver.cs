using System;
using System.Linq;
using Autofac;
using AutofacParameter = Autofac.Core.Parameter;

namespace Nameless.IoC.Autofac {

    /// <summary>
    /// Default implementation of <see cref="IServiceResolver"/> using Autofac (https://autofac.org/).
    /// </summary>
    public sealed class ServiceResolver : IServiceResolver {

        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceResolver"/>.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        public ServiceResolver (ILifetimeScope scope) {
            Prevent.ParameterNull (scope, nameof (scope));

            _scope = scope;
        }

        #endregion

        #region IResolver Members

        /// <inheritdoc />
        public object Get (Type serviceType, string name = null, params Parameter[] parameters) {
            Prevent.ParameterNull (serviceType, nameof (serviceType));

            var otherParameters = (!parameters.IsNullOrEmpty () ?
                parameters.Select (_ => {
                    return _.Type != null ?
                        (AutofacParameter) new TypedParameter (_.Type, _.Value) :
                        (AutofacParameter) new NamedParameter (_.Name, _.Value);
                }) :
                Enumerable.Empty<AutofacParameter> ()).ToArray ();

            return !string.IsNullOrWhiteSpace (name) ?
                _scope.ResolveNamed (name, serviceType, otherParameters) :
                _scope.Resolve (serviceType, otherParameters);
        }

        #endregion
    }
}