using System;
using System.Collections.Generic;

namespace Nameless.PubSub {
    public abstract class PubSubBase : IDisposable {
        #region Protected Static Read-Only Fields

        protected static readonly object SyncLock = new object ();
        protected static readonly IDictionary<string, IList<Subscription>> Cache = new Dictionary<string, IList<Subscription>> ();

        #endregion

        #region Public Properties

        public IEnumerable<string> Topics {
            get { return Cache.Keys; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose () {

        }

        #endregion
    }
}