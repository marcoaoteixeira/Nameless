using System.Data;
using Microsoft.Extensions.Logging;
using Nameless.Data.Requests;
using Nameless.Data.Responses;
using Nameless.ObjectModel;

namespace Nameless.Data;

/// <summary>
///     Default implementation of <see cref="IDatabase" />.
/// </summary>
public class Database : IDatabase, IDisposable {
    private const string LOG_TAG = "DATABASE";

    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<Database> _logger;

    private IDbConnection? _dbConnection;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of <see cref="Database" />.
    /// </summary>
    /// <param name="dbConnectionFactory">The database connection factory.</param>
    /// <param name="logger">The logger.</param>
    public Database(IDbConnectionFactory dbConnectionFactory, ILogger<Database> logger) {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel) {
        BlockAccessAfterDispose();

        return GetDbConnection().BeginTransaction(isolationLevel);
    }

    /// <inheritdoc />
    public ExecuteNonQueryResponse ExecuteNonQuery(ExecuteNonQueryRequest request) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(request.Text, request.Type, request.Parameters);

        try { return command.ExecuteNonQuery(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex, tag: LOG_TAG);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public ExecuteReaderResponse<TResult> ExecuteReader<TResult>(ExecuteReaderRequest<TResult> request) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(request.Text, request.Type, request.Parameters);

        try {
            var reader = command.ExecuteReader(); 
            var result = new List<TResult>();

            using (reader) {
                while (reader.Read()) {
                    result.Add(request.Mapper(reader));
                }
            }

            return result.ToArray();
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex, tag: LOG_TAG);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public ExecuteScalarResponse<TResult> ExecuteScalar<TResult>(ExecuteScalarRequest request) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(request.Text, request.Type, request.Parameters);

        try { return (TResult?)command.ExecuteScalar(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex, tag: LOG_TAG);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        GC.SuppressFinalize(this);
        Dispose(disposing: true);
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~Database() {
        Dispose(disposing: false);
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
        ObjectDisposedException.ThrowIf(_disposed, this);
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
                ConvertParameter(command, parameter)
            );
        }

        Log.OutputDbCommandForDebug(
            _logger,
            command.CommandText,
            command.Parameters
        );

        return command;
    }
}