using System;
using Autofac;

namespace Nameless.DependencyInjection.Autofac {

    /// <summary>
    /// Default implementation of <see cref="ICompositionRoot"/> using Autofac (https://autofac.org/).
    /// </summary>
    public sealed class CompositionRoot : ICompositionRoot, IDisposable {

        #region Private Fields

        private ServiceComposer _currentServiceComposer;
        private IContainer _currentContainer;

        private bool _disposed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Autofac container.
        /// </summary>
        public IContainer Container => _currentContainer;

        #endregion

        #region Public Constructors

        public CompositionRoot () {
            _currentServiceComposer = new ServiceComposer ();
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CompositionRoot () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) { TearDown (); }
            _disposed = true;
        }

        #endregion

        #region ICompositionRoot Members

        /// <inheritdoc/>
        public void Compose (Action<IServiceComposer> action) => action (_currentServiceComposer);

        /// <inheritdoc/>
        public void StartUp () {
            if (_currentContainer != null) {
                throw new InvalidOperationException ("Composition root already started.");
            }
            _currentContainer = _currentServiceComposer.Builder.Build ();
        }

        /// <inheritdoc/>
        public void TearDown () {
            if (_currentContainer != null) {
                _currentContainer.Dispose ();
            }
            _currentContainer = null;
            _currentServiceComposer = null;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc/>
        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (obj: this);
        }

        #endregion
    }
}