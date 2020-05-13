using System;
using System.Data;

namespace Nameless.Data {
    public sealed class DbTransactionWrapper : IDbTransaction, IDisposable {
        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Properties

        public IDbTransaction CurrentTransaction { get; private set; }
        public DbTransactionState State { get; private set; }

        #endregion

        #region Public Constructors

        public DbTransactionWrapper (IDbConnection connection, IsolationLevel level = IsolationLevel.Unspecified) {
            Prevent.ParameterNull (connection, nameof (connection));

            CurrentTransaction = connection.BeginTransaction (level);
        }

        #endregion

        #region Destructor

        ~DbTransactionWrapper () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (CurrentTransaction != null) {
                    if (State == DbTransactionState.None) {
                        CurrentTransaction.Rollback ();
                        State = DbTransactionState.Rolledback;
                    }
                    CurrentTransaction.Dispose ();
                }
            }

            CurrentTransaction = null;
            _disposed = true;
        }

        #endregion

        #region IDbTransaction Members

        public IDbConnection Connection => CurrentTransaction.Connection;

        public IsolationLevel IsolationLevel => CurrentTransaction.IsolationLevel;

        public void Commit () {
            CurrentTransaction.Commit ();
            State = DbTransactionState.Committed;
        }

        public void Rollback () {
            CurrentTransaction.Rollback ();
            State = DbTransactionState.Rolledback;
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