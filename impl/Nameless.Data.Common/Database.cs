using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Nameless.Logging;

namespace Nameless.Data {

    /// <summary>
    /// Default implementation of <see cref="IDatabase"/>.
    /// </summary>
    public sealed class Database : IDatabase {

        #region Private Read-Only Fields

        private readonly IDbConnectionFactory _factory;

        #endregion

        #region Private Fields

        private IDbConnection _connection;
        private DbTransactionWrapper _transaction;
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger _logger;
        /// <summary>
        /// Gets or sets the logger instance.
        /// </summary>
        public ILogger Logger {
            get { return _logger ?? NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="factory">The database connection factory.</param>
        public Database (IDbConnectionFactory factory) {
            Prevent.ParameterNull (factory, nameof (factory));

            _factory = factory;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Database () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static void LogCommand (ILogger logger, IDbCommand command, Parameter[] parameters) {
            if (!logger.IsEnabled (LogLevel.Information)) { return; }

            var information = new StringBuilder ()
                .AppendLine ($"Command being executed: {command.CommandText}")
                .AppendLine ($"\t*** Parameters");

            parameters.Each (_ => information.AppendLine (_.ToString ()));

            logger.Information (information.ToString ());
        }

        #endregion

        #region Private Methods

        private IDbConnection GetConnection () {
            try {
                if (_connection == null) {
                    _connection = _factory.Create ();

                    if (_connection.State == ConnectionState.Closed) {
                        _connection.Open ();
                    }
                }
            } catch (Exception ex) { Logger.Error (ex, ex.Message); throw; }

            return _connection;
        }

        private IDbDataParameter ConvertParameter (IDbCommand command, Parameter parameter) {
            var result = command.CreateParameter ();
            result.ParameterName = (!parameter.Name.StartsWith ("@") ? string.Concat ("@", parameter.Name) : parameter.Name);
            result.DbType = parameter.Type;
            result.Direction = parameter.Direction;
            result.Value = (parameter.Value ?? DBNull.Value);
            return result;
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (nameof (Database));
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                // Rolls back uncommited transaction.
                if (_transaction != null) {
                    try { _transaction.Dispose (); }
                    catch (Exception ex) { Logger.Error (ex, "Error on Transaction Dispose event."); } // On error, just swallow and log.
                }

                if (_connection != null) {
                    try {
                        if (_connection.State == ConnectionState.Open) {
                            _connection.Close ();
                        }
                        _connection.Dispose ();
                    } catch (Exception ex) { Logger.Error (ex, "Error on Database Close event."); } // On error, just swallow and log.
                }
            }

            _connection = null;
            _transaction = null;
            _disposed = true;
        }

        private void PrepareCommand (IDbCommand command, string commandText, CommandType commandType, Parameter[] parameters) {
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (_transaction?.State == DbTransactionState.None) {
                command.Transaction = _transaction.CurrentTransaction;
            }

            parameters.Each (parameter => command.Parameters.Add (ConvertParameter (command, parameter)));
        }

        private Task<object> ExecuteAsync (string commandText, CommandType commandType, Parameter[] parameters, bool scalar, CancellationToken token = default) {
            using (var command = GetConnection ().CreateCommand ()) {
                try {
                    PrepareCommand (command, commandText, commandType, parameters);

                    token.ThrowIfCancellationRequested ();

                    var result = scalar ?
                        command.ExecuteScalar () :
                        command.ExecuteNonQuery ();

                    command.Parameters.OfType<DbParameter> ()
                        .Where (dbParameter => dbParameter.Direction != ParameterDirection.Input)
                        .Each (dbParameter => {
                            parameters
                                .Single (parameter =>
                                    parameter.Name == dbParameter.ParameterName &&
                                    parameter.Direction == dbParameter.Direction)
                                .Value = dbParameter.Value;
                        });

                    return Task.FromResult (result);
                } catch (Exception ex) { Logger.Error (ex, ex.Message); LogCommand (Logger, command, parameters); throw; }
            }
        }

        #endregion

        #region IDatabase Members

        /// <inheritdoc/>
        public string ProviderName {
            get { return _factory.ProviderName; }
        }

        /// <inheritdoc/>
        public IDbTransaction StartTransaction (IsolationLevel level = IsolationLevel.Unspecified) {
            if (_transaction?.State != DbTransactionState.None) {
                // Dispose used transaction
                if (_transaction != null) {
                    _transaction.Dispose ();
                    _transaction = null;
                }

                _transaction = new DbTransactionWrapper (GetConnection (), level);
            }
            return _transaction;
        }

        /// <inheritdoc/>
        public Task<int> ExecuteNonQueryAsync (string commandText, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters) {
            BlockAccessAfterDispose ();

            return ExecuteAsync (commandText, commandType, parameters, false /* scalar */ , token)
                .ContinueWith (continuation => continuation.CanContinue () ? (int) continuation.Result : -1);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TResult> ExecuteReaderAsync<TResult> (string commandText, Func<IDataRecord, TResult> mapper, CommandType commandType = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose ();

            return new AsyncEnumerable<TResult> (async item => {
                using (var command = GetConnection ().CreateCommand ()) { 
                    PrepareCommand (command, commandText, commandType, parameters);
                    item.CancellationToken.ThrowIfCancellationRequested ();

                    IDataReader reader;
                    try { reader = command.ExecuteReader (); }
                    catch (Exception ex) { Logger.Error (ex, ex.Message); LogCommand (Logger, command, parameters); throw; }
                    using (reader) {
                        while (reader.Read ()) {
                            item.CancellationToken.ThrowIfCancellationRequested ();
                            var record = mapper (reader);
                            await item.ReturnAsync (record);
                        }
                    }
                }
            });
        }

        /// <inheritdoc/>
        public Task<object> ExecuteScalarAsync (string commandText, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters) {
            BlockAccessAfterDispose ();

            return ExecuteAsync (commandText, commandType, parameters, true /* scalar */ , token);
        }

        #endregion IDatabase Members

        #region IDisposable Members

        /// <inheritdoc/>
        public void Dispose () {
            GC.SuppressFinalize (this);
            Dispose (true);
        }

        #endregion IDisposable Members
    }
}