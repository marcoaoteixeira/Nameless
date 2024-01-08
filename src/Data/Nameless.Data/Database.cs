using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data {
    /// <summary>
    /// Default implementation of <see cref="IDatabase"/>.
    /// </summary>
    public sealed class Database : IDatabase, IDisposable {
        #region Private Read-Only Fields

        private readonly IDbConnection _dbConnection;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Private Properties

        private IDbTransaction? _dbTransaction;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="dbConnection">The database connection.</param>
        public Database(IDbConnection dbConnection)
            : this(dbConnection, NullLogger.Instance) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="dbConnection">The database connection.</param>
        /// <param name="logger">The logger.</param>
        public Database(IDbConnection dbConnection, ILogger logger) {
            _dbConnection = Guard.Against.Null(dbConnection, nameof(dbConnection));
            _logger = Guard.Against.Null(logger, nameof(logger));

            _dbConnection.EnsureOpen();
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
            result.ParameterName = parameter.Name;
            result.DbType = parameter.Type;
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
                _dbTransaction?.Rollback();
                _dbTransaction?.Dispose();
            }

            _dbTransaction = null;
            _disposed = true;
        }

        private IDbCommand CreateCommand(string text, CommandType type, Parameter[] parameters) {
            var command = _dbConnection.CreateCommand();
            command.CommandText = text;
            command.CommandType = type;
            command.Transaction = _dbTransaction;

            foreach (var parameter in parameters) {
                command.Parameters.Add(
                    value: ConvertParameter(command, parameter)
                );
            }

            _logger.DbCommand(command);

            return command;
        }

        #endregion

        #region IDatabase Members

        /// <inheritdoc/>
        public void StartTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) {
            BlockAccessAfterDispose();

            _dbTransaction ??= _dbConnection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction() {
            BlockAccessAfterDispose();

            _dbTransaction?.Commit();
            _dbTransaction = null;
        }

        public void RollbackTransaction() {
            BlockAccessAfterDispose();

            _dbTransaction?.Rollback();
            _dbTransaction = null;
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return command.ExecuteNonQuery(); }
            catch (Exception ex) {
                _logger.LogError(
                    exception: ex,
                    message: "Error while executing non-query. {Message}",
                    args: ex.Message
                );
                throw;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            IDataReader reader;
            try { reader = command.ExecuteReader(); }
            catch (Exception ex) {
                _logger.LogError(
                    exception: ex,
                    message: "Error while executing reader. {Message}",
                    args: ex.Message
                );
                throw;
            }
            using (reader) {
                while (reader.Read()) {
                    yield return mapper(reader);
                }
            }
        }

        /// <inheritdoc/>
        public TResult? ExecuteScalar<TResult>(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return (TResult?)command.ExecuteScalar(); }
            catch (Exception ex) {
                _logger.LogError(
                    exception: ex,
                    message: "Error while executing scalar. {Message}",
                    args: ex.Message
                );
                throw;
            }
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