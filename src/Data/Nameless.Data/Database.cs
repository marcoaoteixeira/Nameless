using System.Data;
using Nameless.Logging;

namespace Nameless.Data {

    /// <summary>
    /// Default implementation of <see cref="IDatabase"/>.
    /// </summary>
    public sealed class Database : IDatabase, IDisposable {

        #region Private Fields

        private IDbConnection? _connection;
        private DbTransactionWrapper? _transaction;
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger _logger = default!;
        /// <summary>
        /// Gets or sets the logger instance.
        /// </summary>
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        public Database(IDbConnectionFactory dbConnectionFactory) {
            Prevent.Null(dbConnectionFactory, nameof(dbConnectionFactory));

            _connection = dbConnectionFactory.Create();

            // Ensure connection is open.
            if (_connection.State != ConnectionState.Open) {
                _connection.Open();
            }
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Database() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static IDbDataParameter ConvertParameter(IDbCommand command, Parameter parameter) {
            var result = command.CreateParameter();
            result.ParameterName = !parameter.Name.StartsWith("@") ? string.Concat("@", parameter.Name) : parameter.Name;
            result.DbType = parameter.Type;
            result.Direction = parameter.Direction;
            result.Value = parameter.Value ?? DBNull.Value;
            return result;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(Database));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _transaction?.Dispose();
                _connection?.Dispose();
            }

            _transaction = default;
            _connection = default;
            _disposed = true;
        }

        private void DbTransactionWrapperConsumed(object sender, EventArgs e) {
            _transaction = default;
        }

        private IDbCommand CreateCommand(string commandText, CommandType commandType, Parameter[] parameters) {
            BlockAccessAfterDispose();

            if (_connection == default) { throw new InvalidOperationException("Database connection missing."); }

            var command = _connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = _transaction;

            parameters.Each(parameter => command.Parameters.Add(ConvertParameter(command, parameter)));

            Logger.Debug(command);

            return command;
        }

        private TResult? Execute<TResult>(string commandText, CommandType commandType, Parameter[] parameters, bool scalar) {
            using var command = CreateCommand(commandText, commandType, parameters);
            try {
                var result = scalar ?
                    command.ExecuteScalar() :
                    command.ExecuteNonQuery();

                command.Parameters
                    .OfType<IDbDataParameter>()
                    .Where(dbParameter => dbParameter.Direction != ParameterDirection.Input)
                    .Each(dbParameter => {
                        parameters
                            .Single(parameter =>
                               parameter.Name == dbParameter.ParameterName &&
                               parameter.Direction == dbParameter.Direction
                            ).Value = dbParameter.Value;
                    });

                return (TResult?)result;
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
        }

        #endregion

        #region IDatabase Members

        /// <inheritdoc/>
        public IDbTransaction StartTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) {
            if (_connection == default) { throw new InvalidOperationException("Database connection missing."); }

            if (_transaction == default) {
                _transaction = new DbTransactionWrapper(_connection, isolationLevel);
                _transaction.Committed += DbTransactionWrapperConsumed!;
                _transaction.Rolledback += DbTransactionWrapperConsumed!;
                _transaction.Disposed += DbTransactionWrapperConsumed!;
            }
            return _transaction;
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, params Parameter[] parameters) {
            Prevent.NullOrWhiteSpaces(commandText, nameof(commandText));

            BlockAccessAfterDispose();

            return Execute<int>(commandText, commandType, parameters, scalar: false);
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string commandText, Func<IDataRecord, TResult> mapper, CommandType commandType = CommandType.Text, params Parameter[] parameters) {
            Prevent.NullOrWhiteSpaces(commandText, nameof(commandText));

            BlockAccessAfterDispose();

            using var command = CreateCommand(commandText, commandType, parameters);

            

            IDataReader reader;
            try { reader = command.ExecuteReader(); } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
            using (reader) {
                while (reader.Read()) {
                    yield return mapper(reader);
                }
            }
        }

        /// <inheritdoc/>
        public TResult? ExecuteScalar<TResult>(string commandText, CommandType commandType = CommandType.Text, params Parameter[] parameters) {
            Prevent.NullOrWhiteSpaces(commandText, nameof(commandText));

            BlockAccessAfterDispose();

            return Execute<TResult>(commandText, commandType, parameters, scalar: true);
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc/>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion
    }
}