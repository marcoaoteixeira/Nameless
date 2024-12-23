﻿using System.Data;
using Microsoft.Extensions.Logging;
using Nameless.Data.Internals;

namespace Nameless.Data;

/// <summary>
/// Default implementation of <see cref="IDatabase"/>.
/// </summary>
public sealed class Database : IDatabase, IDisposable {
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<Database> _logger;

    private IDbConnection? _dbConnection;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="Database"/>.
    /// </summary>
    /// <param name="dbConnectionFactory">The database connection factory.</param>
    /// <param name="logger">The logger.</param>
    public Database(IDbConnectionFactory dbConnectionFactory, ILogger<Database> logger) {
        _dbConnectionFactory = Prevent.Argument.Null(dbConnectionFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    ~Database() {
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
        catch (Exception ex) { _logger.ErrorOnExecuteNonQueryCommand(ex); throw; }
    }

    /// <inheritdoc/>
    public IEnumerable<TResult> ExecuteReader<TResult>(string text, CommandType type, Func<IDataRecord, TResult> mapper, params Parameter[] parameters) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(text);

        using var command = CreateCommand(text, type, parameters);

        IDataReader reader;
        try { reader = command.ExecuteReader(); }
        catch (Exception ex) { _logger.ErrorOnExecuteReaderCommand(ex); throw; }
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
        catch (Exception ex) { _logger.ErrorOnExecuteScalarCommand(ex); throw; }
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
            throw new ObjectDisposedException(nameof(Database));
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

        _logger.OutputDbCommand(command);

        return command;
    }
}