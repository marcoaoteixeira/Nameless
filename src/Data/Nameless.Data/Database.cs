using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data {
    /// <summary>
    /// Default implementation of <see cref="IDatabase"/>.
    /// </summary>
    public sealed class Database : IDatabase, IDisposable {
        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger _logger = default!;
        /// <summary>
        /// Gets or sets the logger instance.
        /// </summary>
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Private Properties

        private IDbConnection _connection;
        private IDbTransaction? _transaction;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        public Database(IDbConnectionFactory dbConnectionFactory) {
            Prevent.Against.Null(dbConnectionFactory, nameof(dbConnectionFactory));

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
                _transaction?.Dispose();
                _connection?.Dispose();
            }

            _transaction = null!;
            _connection = null!;
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
            _transaction = null!;
        }

        public void RollbackTransaction() {
            BlockAccessAfterDispose();

            _transaction?.Rollback();
            _transaction = null!;
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Prevent.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return command.ExecuteNonQuery(); }
            catch (Exception ex) { Logger.LogError(ex, ex.Message); throw; }
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Prevent.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            IDataReader reader;
            try { reader = command.ExecuteReader(); }
            catch (Exception ex) { Logger.LogError(ex, ex.Message); throw; }
            using (reader) {
                while (reader.Read()) {
                    yield return mapper(reader);
                }
            }
        }

        /// <inheritdoc/>
        public TResult? ExecuteScalar<TResult>(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Prevent.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return (TResult?)command.ExecuteScalar(); }
            catch (Exception ex) { Logger.LogError(ex, ex.Message); throw; }
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