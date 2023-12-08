using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data {
    /// <summary>
    /// Default implementation of <see cref="IDatabase"/>.
    /// </summary>
    public sealed class Database : IDatabase, IDisposable {
        #region Private Read-Only Fields

        private readonly IDbConnection _connection;

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Private Properties

        private IDbTransaction? _transaction;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        public Database(IDbConnection connection) {
            _connection = Guard.Against.Null(connection, nameof(connection));
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

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(Database));

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _transaction?.Rollback();
                _transaction?.Dispose();
            }

            _transaction = null;
            _disposed = true;
        }

        private IDbCommand CreateCommand(string text, CommandType type, Parameter[] parameters) {
            var command = _connection.CreateCommand();
            command.CommandText = text;
            command.CommandType = type;
            command.Transaction = _transaction;

            foreach (var parameter in parameters) {
                command.Parameters.Add(
                    value: ConvertParameter(command, parameter)
                );
            }

            Logger.DbCommand(command);

            return command;
        }

        #endregion

        #region IDatabase Members

        /// <inheritdoc/>
        public void StartTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) {
            BlockAccessAfterDispose();

            _transaction ??= _connection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction() {
            BlockAccessAfterDispose();

            _transaction?.Commit();
            _transaction = null;
        }

        public void RollbackTransaction() {
            BlockAccessAfterDispose();

            _transaction?.Rollback();
            _transaction = null;
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return command.ExecuteNonQuery(); } catch (Exception ex) { Logger.LogError(ex, "{ex.Message}", ex); throw; }
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            IDataReader reader;
            try { reader = command.ExecuteReader(); } catch (Exception ex) { Logger.LogError(ex, "{ex.Message}", ex); throw; }
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

            try { return (TResult?)command.ExecuteScalar(); } catch (Exception ex) { Logger.LogError(ex, "{ex.Message}", ex); throw; }
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