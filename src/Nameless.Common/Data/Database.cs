using System.Data;
using Microsoft.Extensions.Logging;
using Nameless.ObjectModel;

namespace Nameless.Data;

/// <summary>
///     Default implementation of <see cref="IDatabase" />.
/// </summary>
public class Database : IDatabase, IDisposable {
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
    /// <exception cref="ArgumentException">
    ///     if <paramref name="sql"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="sql"/> or <paramref name="parameters"/> is
    ///     <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     if the current instance of <see cref="Database"/> class
    ///     is disposed.
    /// </exception>
    public ExecuteNonQueryResult ExecuteNonQuery(string sql, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(sql, type, parameters);

        try { return command.ExecuteNonQuery(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="sql"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="sql"/> or <paramref name="parameters"/> is
    ///     <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     if the current instance of <see cref="Database"/> class
    ///     is disposed.
    /// </exception>
    public ExecuteReaderResult<T> ExecuteReader<T>(string sql, Func<IDataRecord, T> mapper, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(sql, type, parameters);

        try {
            var reader = command.ExecuteReader(); 
            var result = new List<T>();

            using (reader) {
                while (reader.Read()) {
                    result.Add(mapper(reader));
                }
            }

            return result.ToArray();
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="sql"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="sql"/> or <paramref name="parameters"/> is
    ///     <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     if the current instance of <see cref="Database"/> class
    ///     is disposed.
    /// </exception>
    public ExecuteScalarResult<T> ExecuteScalar<T>(string sql, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters) {
        BlockAccessAfterDispose();

        using var command = CreateCommand(sql, type, parameters);

        try { return (T?)command.ExecuteScalar(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
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

    private IDbCommand CreateCommand(string sql, CommandType type, IEnumerable<Parameter> parameters) {
        Throws.When.NullOrWhiteSpace(sql);
        Throws.When.Null(parameters);

        var command = GetDbConnection().CreateCommand();

        command.CommandText = sql;
        command.CommandType = type;

        foreach (var parameter in parameters) {
            command.Parameters.Add(
                ConvertParameter(command, parameter)
            );
        }

        Log.OutputDbCommandForDebug(
            _logger,
            command.CommandText,
            command.Parameters,
            tag: GetType().Tag
        );

        return command;
    }
}