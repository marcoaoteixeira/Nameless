using System;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public sealed class AnonymousDisposable : IDisposable {
        #region Private Read-Only Fields

        private readonly Action _action;

        #endregion

        #region Public Constructors

        public AnonymousDisposable (Action action) {
            Prevent.ParameterNull (action, nameof (action));

            _action = action;
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            _action ();
        }

        #endregion
    }
}