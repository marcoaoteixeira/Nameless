﻿using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data {
    /// <summary>
    /// Default implementation of <see cref="IDatabaseService"/>.
    /// </summary>
    public sealed class DatabaseService : IDatabaseService, IDisposable {
        #region Private Read-Only Fields

        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private IDbConnection? _dbConnection;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseService"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        public DatabaseService(IDbConnectionFactory dbConnectionFactory)
            : this(dbConnectionFactory, NullLogger<DatabaseService>.Instance) { }

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseService"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        /// <param name="logger">The logger.</param>
        public DatabaseService(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseService> logger) {
            _dbConnectionFactory = Guard.Against.Null(dbConnectionFactory, nameof(dbConnectionFactory));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Destructor

        ~DatabaseService() {
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

        private IDbConnection GetDbConnection() {
            if (_dbConnection is null) {
                _dbConnection = _dbConnectionFactory.CreateDbConnection();
                _dbConnection.EnsureOpen();
            }

            return _dbConnection;
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(DatabaseService));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _dbConnection?.Dispose();
            }

            _dbConnection = null;
            _disposed = true;
        }

        private IDbCommand CreateCommand(string text, CommandType type, IEnumerable<Parameter> parameters) {
            var command = GetDbConnection().CreateCommand();
            command.CommandText = text;
            command.CommandType = type;

            foreach (var parameter in parameters) {
                command.Parameters.Add(
                    value: ConvertParameter(command, parameter)
                );
            }

            _logger.DbCommand(command);

            return command;
        }

        #endregion

        #region IDatabaseService Members

        /// <inheritdoc/>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel) {
            BlockAccessAfterDispose();

            return GetDbConnection().BeginTransaction(isolationLevel);
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string text, CommandType type, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return command.ExecuteNonQuery(); }
            catch (Exception ex) {
                _logger.LogError(exception: ex,
                                 message: "Error while executing non-query. {Message}",
                                 args: ex.Message);
                throw;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            IDataReader reader;
            try { reader = command.ExecuteReader(); }
            catch (Exception ex) {
                _logger.LogError(exception: ex,
                                 message: "Error while executing reader. {Message}",
                                 args: ex.Message);
                throw;
            }
            using (reader) {
                while (reader.Read()) {
                    yield return mapper(reader);
                }
            }
        }

        /// <inheritdoc/>
        public TResult? ExecuteScalar<TResult>(string text, CommandType type, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(text, nameof(text));

            using var command = CreateCommand(text, type, parameters);

            try { return (TResult?)command.ExecuteScalar(); }
            catch (Exception ex) {
                _logger.LogError(exception: ex,
                                 message: "Error while executing scalar. {Message}",
                                 args: ex.Message);
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