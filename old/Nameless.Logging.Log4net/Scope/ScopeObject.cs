using System;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Scope {
    public sealed class ScopeObject : IDisposable {

        #region Private Read-Only Fields

        private readonly Stack<IDisposable> _disposables = new Stack<IDisposable> ();

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Constructors

        public ScopeObject (object scope, ScopeRegistry registry) {
            Prevent.ParameterNull (scope, nameof (scope));
            Prevent.ParameterNull (registry, nameof (registry));

            var scopeType = scope.GetType ();
            var register = registry.GetRegister (scopeType);
            foreach (var item in register (scope)) {
                _disposables.Push (item);
            }
        }

        #endregion

        #region Destructors

        ~ScopeObject () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                while (_disposables.Count > 0) {
                    _disposables.Pop ().Dispose ();
                }
            }

            _disposables.Clear ();
            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}