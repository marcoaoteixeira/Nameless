﻿using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Infrastructure;
using Nameless.IO;
using Nameless.Lucene.Options;
using Polly;
using Polly.Retry;

namespace Nameless.Lucene;

/// <summary>
/// Default implementation of <see cref="IIndexProvider"/>
/// </summary>
public sealed class IndexProvider : IIndexProvider, IDisposable {
    private const int MAX_DELETE_INDEX_RETRY_ATTEMPTS = 3;
    private const int THRESHOLD_TIME = 2000; // 2 seconds

    private readonly IApplicationContext _applicationContext;
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly IFileSystem _fileSystem;
    private readonly IOptions<LuceneOptions> _options;
    private readonly ILogger<Index> _loggerForIndex;
    private readonly RetryPolicy _deleteIndexRetryPolicy;

    private ConcurrentDictionary<string, IIndex> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="IndexProvider"/>.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="analyzerProvider">The analyzer provider.</param>
    /// <param name="loggerForIndex">The <see cref="ILogger{Index}"/> that will be passed to the Index when created.</param>
    /// <param name="fileSystem">The file system services.</param>
    /// <param name="options">The settings.</param>
    public IndexProvider(IApplicationContext applicationContext,
                         IAnalyzerProvider analyzerProvider,
                         IFileSystem fileSystem,
                         IOptions<LuceneOptions> options,
                         ILogger<Index> loggerForIndex) {
        _applicationContext = Prevent.Argument.Null(applicationContext);
        _analyzerProvider = Prevent.Argument.Null(analyzerProvider);
        _fileSystem = Prevent.Argument.Null(fileSystem);
        _loggerForIndex = Prevent.Argument.Null(loggerForIndex);
        _options = Prevent.Argument.Null(options);

        // In case of IOException, wait and try again.
        // Time will increase over attempts.
        _deleteIndexRetryPolicy = Policy.Handle<IOException>()
                                        .WaitAndRetry(retryCount: MAX_DELETE_INDEX_RETRY_ATTEMPTS,
                                                      sleepDurationProvider: retry => TimeSpan.FromMilliseconds(retry * THRESHOLD_TIME));
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public void DeleteIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        _deleteIndexRetryPolicy.Execute(() => {
            var indexDirectoryPath = GetIndexDirectoryPath(name);

            if (!_fileSystem.Directory.Exists(indexDirectoryPath)) {
                return;
            }

            _fileSystem.Directory.Delete(directoryPath: indexDirectoryPath,
                                         recursive: true);

            if (Cache.TryRemove(name, out var index) &&
                index is IDisposable disposable) {
                disposable.Dispose();
            }
        });
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public bool IndexExists(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        var indexDirectoryPath = GetIndexDirectoryPath(name);

        return _fileSystem.Directory.Exists(indexDirectoryPath);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public IIndex CreateIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        return Cache.GetOrAdd(name, Create);
    }

    /// <inheritdoc />
    public IEnumerable<string> ListIndexes() {
        BlockAccessAfterDispose();

        var rootPath = _fileSystem.Path.Combine(_applicationContext.AppDataFolderPath,
                                                _options.Value.IndexesFolderName);
        var directories = _fileSystem.Directory.GetDirectories(rootPath);

        foreach (var directory in directories) {
            var indexName = _fileSystem.Path.GetFileName(directory.Path);
            if (indexName is not null) {
                yield return indexName;
            }
        }
    }

    public void Dispose()
        => Dispose(disposing: true);

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(IndexProvider));
        }
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var indexes = Cache.Values.ToArray();
            foreach (var index in indexes) {
                if (index is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            Cache.Clear();
        }

        _disposed = true;
    }

    private string GetIndexDirectoryPath(string indexName) {
        var path = _fileSystem.Path.Combine(_applicationContext.AppDataFolderPath,
                                            _options.Value.IndexesFolderName,
                                            indexName);

        return PathHelper.Normalize(path);
    }

    private Index Create(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);
        var directory = _fileSystem.Directory.Create(indexDirectoryPath);

        return new Index(analyzer: _analyzerProvider.GetAnalyzer(indexName),
                         indexDirectoryPath: directory.Path,
                         logger: _loggerForIndex);
    }
}