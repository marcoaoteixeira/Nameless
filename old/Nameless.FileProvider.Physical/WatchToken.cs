using System;
using Microsoft.Extensions.Primitives;

namespace Nameless.FileProvider.Physical {
    public sealed class WatchToken : IWatchToken {
        #region Private Read-Only Fields

        private readonly IChangeToken _token;

        #endregion

        #region Private Constructors

        private WatchToken () { }

        #endregion

        #region Internal Constructors

        internal WatchToken (IChangeToken token) {
            Prevent.ParameterNull (token, nameof (token));

            _token = token;
        }

        #endregion

        #region IWatchToken Members

        public bool Changed => _token.HasChanged;

        public bool ActiveCallback => _token.ActiveChangeCallbacks;

        public IDisposable RegisterCallback (Action<object> callback, object state) => _token.RegisterChangeCallback (callback, state);

        #endregion
    }
}