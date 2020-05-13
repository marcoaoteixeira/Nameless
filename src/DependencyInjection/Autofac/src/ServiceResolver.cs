using System;
using System.Linq;
using Autofac;
using AF_Parameter = Autofac.Core.Parameter;

namespace Nameless.DependencyInjection.Autofac {

    /// <summary>
    /// Default implementation of <see cref="IServiceResolver"/> using Autofac (https://autofac.org/).
    /// </summary>
    public sealed class ServiceResolver : IServiceResolver {

        #region Private Fields

        private ILifetimeScope _currentScope;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceResolver"/>.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <param name="isRoot">Whether is root scope or not.</param>
        public ServiceResolver (ILifetimeScope scope, bool isRoot = false) {
            Prevent.ParameterNull (scope, nameof (scope));

            _currentScope = scope;

            IsRoot = isRoot;
        }

        #endregion

        #region Destructor

        ~ServiceResolver () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed || IsRoot) { return; }
            if (disposing) {
                if (_currentScope != null) {
                    _currentScope.Dispose ();
                }
            }
            _currentScope = null;
            _disposed = true;
        }

        #endregion

        #region IResolver Members

        public bool IsRoot { get; }

        /// <inheritdoc />
        public object Get (Type serviceType, string name = null, params Parameter[] parameters) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNull (serviceType, nameof (serviceType));

            var otherParameters = (!parameters.IsNullOrEmpty () ?
                parameters.Select (_ => {
                    return _.Type != null ?
                        (AF_Parameter) new TypedParameter (_.Type, _.Value) :
                        (AF_Parameter) new NamedParameter (_.Name, _.Value);
                }) :
                Enumerable.Empty<AF_Parameter> ()).ToArray ();

            return !string.IsNullOrWhiteSpace (name) ?
                _currentScope.ResolveNamed (name, serviceType, otherParameters) :
                _currentScope.Resolve (serviceType, otherParameters);
        }

        /// <inheritdoc />
        public IServiceResolver GetScoped () {
            BlockAccessAfterDispose ();

            var scope = _currentScope.BeginLifetimeScope ();
            return new ServiceResolver (scope, isRoot : false);
        }

        /// <inheritdoc />
        public IServiceResolver GetScoped (Action<IServiceComposer> compose) {
            BlockAccessAfterDispose ();
            
            var scope = _currentScope.BeginLifetimeScope (builder => {
                var composer = new ServiceComposer (builder);
                compose (composer);
            });
            return new ServiceResolver (scope, isRoot : false);
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            if (!IsRoot) {
                Dispose (disposing: true);
                GC.SuppressFinalize (this);
            }
        }

        #endregion
    }
}