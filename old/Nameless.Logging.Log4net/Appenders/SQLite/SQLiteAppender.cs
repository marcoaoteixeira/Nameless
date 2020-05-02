using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace Nameless.Logging.Log4net.Appenders.SQLite {
    public class SQLiteAppender : BufferingAppenderSkeleton {
        #region Private Fields

        private IDbConnection _dbConnection;
        private IList<SQLiteAppenderParameter> _parameters = new List<SQLiteAppenderParameter> ();

        #endregion

        #region Public Properties

        public string ConnectionString { get; set; }
        public bool UseTransactions { get; set; }
        public string CommandText { get; set; }

        #endregion

        #region Public Constructors

        public SQLiteAppender () { }

        public SQLiteAppender (IDbConnection dbConnection) {
            Prevent.ParameterNull (dbConnection, nameof (dbConnection));

            if (!(dbConnection is SQLiteConnection)) {
                LogLog.Warn (GetType (), "Passed connection is not a SQLite connection type.");
            }

            _dbConnection = dbConnection;
            ConnectionString = _dbConnection.ConnectionString;
        }

        #endregion

        #region Public Methods

        public void AddParameter (SQLiteAppenderParameter parameter) {
            _parameters.Add (parameter);
        }

        #endregion

        #region Public Override Methods

        public override void ActivateOptions () {
            base.ActivateOptions ();

            InitializeDbConnection ();
        }

        #endregion

        #region Protected Override Methods

        protected override void SendBuffer (LoggingEvent[] events) {
            if (_dbConnection == null) { return; }
            if (_dbConnection.State == ConnectionState.Closed) { return; }
            if (string.IsNullOrWhiteSpace (CommandText)) { LogLog.Debug (GetType (), "CommandText for SQLiteAppender not defined."); return; }

            IDbTransaction transaction = null;
            try {
                transaction = UseTransactions ? _dbConnection.BeginTransaction () : null;
                using (var command = _dbConnection.CreateCommand ()) {
                    command.Transaction = transaction;
                    command.Prepare ();
                    command.CommandText = CommandText;
                    foreach (var evt in events) {
                        command.Parameters.Clear ();
                        foreach (var parameter in _parameters) {
                            parameter.Prepare (command);
                            parameter.FormatValue (command, evt);
                        }
                        command.ExecuteNonQuery ();
                    }
                }
                transaction.Commit ();
            } catch (Exception ex) {
                if (transaction != null) {
                    try { transaction.Rollback (); } catch { /* Ignore this exception */ }
                }
                ErrorHandler.Error ("Exception while writing to database", ex);
            } finally {
                if (transaction != null) { transaction.Dispose (); }
            }
        }

        protected override void OnClose () {
            base.OnClose ();
            DisposeDbConnection ();
        }

        #endregion

        #region Private Methods

        private void InitializeDbConnection () {
            try {
                _dbConnection = _dbConnection ?? new SQLiteConnection (ConnectionString);
                if (_dbConnection.State != ConnectionState.Open) {
                    _dbConnection.Open ();
                }
            } catch (Exception ex) { ErrorHandler.Error ("Could not open database connection [{ConnectionString}].", ex); }
        }

        private void DisposeDbConnection () {
            if (_dbConnection != null) {
                if (_dbConnection.State != ConnectionState.Closed) {
                    _dbConnection.Close ();
                }
                _dbConnection.Dispose ();
                _dbConnection = null;
            }
        }

        #endregion
    }

    public class SQLiteAppenderParameter {
        #region Private Fields

        private DbType _dbType;
        private bool _inferType = true;

        #endregion

        #region Public Properties

        public string ParameterName { get; set; }
        public DbType DbType {
            get { return _dbType; }
            set {
                _dbType = value;
                _inferType = false;
            }
        }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public int Size { get; set; }
        public IRawLayout Layout { get; set; }

        #endregion

        #region Public Constructors

        public SQLiteAppenderParameter () {
            Precision = 0;
            Scale = 0;
            Size = 0;
        }

        #endregion

        #region Public Methods

        public void Prepare (IDbCommand command) {
            var parameter = command.CreateParameter ();
            parameter.ParameterName = ParameterName;
            if (!_inferType) { parameter.DbType = DbType; }
            if (Precision != 0) { parameter.Precision = Precision; }
            if (Scale != 0) { parameter.Scale = Scale; }
            if (Size != 0) { parameter.Size = Size; }
            command.Parameters.Add (parameter);
        }

        public void FormatValue (IDbCommand command, LoggingEvent evt) {
            // Lookup the parameter
            var parameter = (IDbDataParameter)command.Parameters[ParameterName];

            // Format the value
            var value = Layout.Format (evt);

            // If the value is null then convert to a DBNull
            parameter.Value = value != null ? value : DBNull.Value;
        }

        #endregion
    }
}