using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Helpers {

    /// <summary>
    /// Asynchronous helper.
    /// </summary>
    public static class AsyncHelper {

        #region Public Static Methods

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <param name="function">The async method.</param>
        public static void RunSync (Func<Task> function) {
            var currentContext = SynchronizationContext.Current;
            var exclusiveContext = new ExclusiveSynchronizationContext ();
            SynchronizationContext.SetSynchronizationContext (exclusiveContext);
            exclusiveContext.Post (async _ => {
                try { await function (); }
                finally { exclusiveContext.EndMessageLoop (); }
            }, state : null);
            exclusiveContext.BeginMessageLoop ();

            SynchronizationContext.SetSynchronizationContext (currentContext);
        }

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="function">The async method.</param>
        public static T RunSync<T> (Func<Task<T>> function) {
            var currentContext = SynchronizationContext.Current;
            var exclusiveContext = new ExclusiveSynchronizationContext ();
            SynchronizationContext.SetSynchronizationContext (exclusiveContext);
            T result = default;
            exclusiveContext.Post (async _ => {
                try { result = await function (); }
                finally { exclusiveContext.EndMessageLoop (); }
            }, state : null);
            exclusiveContext.BeginMessageLoop ();
            SynchronizationContext.SetSynchronizationContext (currentContext);
            return result;
        }

        #endregion

        #region Private Inner Classes

        private class ExclusiveSynchronizationContext : SynchronizationContext {

            #region Private Fields

            private bool _done;

            #endregion

            #region Private Read-Only Fields

            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent (false);
            private readonly Queue<Tuple<SendOrPostCallback, object>> _items = new Queue<Tuple<SendOrPostCallback, object>> ();

            #endregion

            #region Public Methods

            public void BeginMessageLoop () {
                while (!_done) {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (_items) {
                        if (_items.Count > 0) {
                            task = _items.Dequeue ();
                        }
                    }
                    if (task != null) { task.Item1 (task.Item2); } else { _workItemsWaiting.WaitOne (); }
                }
            }

            public void EndMessageLoop () {
                Post (_ => _done = true, null);
            }

            #endregion

            #region Public Override Methods

            /// <inheritdoc />
            public override void Send (SendOrPostCallback callback, object state) {
                throw new NotSupportedException ("We cannot send to our same thread");
            }

            /// <inheritdoc />
            public override void Post (SendOrPostCallback callback, object state) {
                lock (_items) {
                    _items.Enqueue (Tuple.Create (callback, state));
                }
                _workItemsWaiting.Set ();
            }

            /// <inheritdoc />
            public override SynchronizationContext CreateCopy () {
                return this;
            }

            #endregion
        }

        #endregion
    }
}