using System.Data;
using Nameless.Logging;

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

        private IDbConnection? _connection;
        private IDbConnection DbConnection {
            get => _connection ?? throw new NullReferenceException();
            set => _connection = value;
        }

        private IDbTransaction? _transaction;
        private IDbTransaction DbTransaction {
            get => _transaction ?? throw new NullReferenceException();
            set => _transaction = value;
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        public Database(IDbConnectionFactory dbConnectionFactory) {
            Garda.Prevent.Null(dbConnectionFactory, nameof(dbConnectionFactory));

            DbConnection = dbConnectionFactory.Create();

            // Ensure connection is open.
            if (DbConnection.State != ConnectionState.Open) {
                DbConnection.Open();
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
                DbTransaction.Dispose();
                DbConnection.Dispose();
            }

            DbTransaction = null!;
            DbConnection = null!;
            _disposed = true;
        }

        private IDbCommand CreateCommand(string text, CommandType type, Parameter[] parameters) {
            var command = DbConnection.CreateCommand();
            command.CommandText = text;
            command.CommandType = type;
            command.Transaction = _transaction;

            foreach (var parameter in parameters) {
                command.Parameters.Add(
                    value: ConvertParameter(command, parameter)
                );
            }

            Logger.Debug(command);

            return command;
        }

        private TResult? Execute<TResult>(string text, CommandType type, Parameter[] parameters, bool scalar) {
            using var command = CreateCommand(text, type, parameters);
            try {
                var result = scalar ?
                    command.ExecuteScalar() :
                    command.ExecuteNonQuery();

                return (TResult?)result;
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
        }

        #endregion

        #region IDatabase Members

        /// <inheritdoc/>
        public void StartTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) {
            BlockAccessAfterDispose();

            DbTransaction ??= DbConnection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction() {
            BlockAccessAfterDispose();

            DbTransaction.Commit();

            _transaction = null;
        }

        public void RollbackTransaction() {
            BlockAccessAfterDispose();

            DbTransaction.Rollback();

            _transaction = null;
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Garda.Prevent.NullOrWhiteSpace(text, nameof(text));

            return Execute<int>(text, type, parameters, scalar: false);
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Garda.Prevent.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            IDataReader reader;
            try { reader = command.ExecuteReader(); }
            catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
            using (reader) {
                while (reader.Read()) {
                    yield return mapper(reader);
                }
            }
        }

        /// <inheritdoc/>
        public TResult? ExecuteScalar<TResult>(string text, CommandType type = CommandType.Text, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Garda.Prevent.NullOrWhiteSpace(text, nameof(text));

            return Execute<TResult>(text, type, parameters, scalar: true);
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