using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Data.Internals;

namespace Nameless.Data;

/// <summary>
/// Default implementation of <see cref="IDatabaseService"/>.
/// </summary>
public sealed class DatabaseService : IDatabaseService, IDisposable {
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger _logger;

    private IDbConnection? _dbConnection;
    private bool _disposed;

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
        _dbConnectionFactory = Prevent.Argument.Null(dbConnectionFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    ~DatabaseService() {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel) {
        BlockAccessAfterDispose();

        return GetDbConnection().BeginTransaction(isolationLevel);
    }

    /// <inheritdoc/>
    public int ExecuteNonQuery(string text, CommandType type, params Parameter[] parameters) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(text);

        using var command = CreateCommand(text, type, parameters);

        try { return command.ExecuteNonQuery(); }
        catch (Exception ex) {
            LoggerHandlers.ErrorOnNonQueryExecution(_logger, ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type, params Parameter[] parameters) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(text);

        using var command = CreateCommand(text, type, parameters);

        IDataReader reader;
        try { reader = command.ExecuteReader(); }
        catch (Exception ex) {
            LoggerHandlers.ErrorOnReaderExecution(_logger, ex.Message, ex);
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

        Prevent.Argument.NullOrWhiteSpace(text);

        using var command = CreateCommand(text, type, parameters);

        try { return (TResult?)command.ExecuteScalar(); }
        catch (Exception ex) {
            LoggerHandlers.ErrorOnScalarExecution(_logger, ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private static IDbDataParameter ConvertParameter(IDbCommand command, Parameter parameter) {
        var result = command.CreateParameter();
        result.ParameterName = parameter.Name;
        result.DbType = parameter.Type;
        result.Value = parameter.Value ?? DBNull.Value;
        return result;
    }

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

        LoggerHandlers.DebugDbCommand(_logger,
                                    command.CommandText,
                                    command.GetParameterList(),
                                    null /* exception */);

        return command;
    }
}