using System;

namespace Nameless.FileProvider {
    public interface IWatchToken {
        #region Properties

        bool Changed { get; }
        bool ActiveCallback { get; }

        #endregion

        #region Methods

        IDisposable RegisterCallback (Action<object> callback, object state);

        #endregion
    }
}