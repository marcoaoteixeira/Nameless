using System;
using System.Data;
using System.Data.SQLite;

namespace Nameless.EventSourcing.Test {
    public sealed class SQLiteInMemory : IDisposable {
        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Properties

        public IDbConnection DbConnection { get; private set; }

        #endregion

        #region Public Constructors

        public SQLiteInMemory () {
            Initialize ();
        }

        #endregion

        #region Destructor

        ~SQLiteInMemory () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            DbConnection = new SQLiteConnection ("Data Source=:memory:;Version=3;New=True;");
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (nameof (SQLiteInMemory));
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (DbConnection != null) {
                    DbConnection.Close ();
                    DbConnection.Dispose ();
                }
            }

            DbConnection = null;

            _disposed = true;
        }

        #endregion

        #region Disposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion

    }
}
