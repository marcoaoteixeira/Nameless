using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Configuration;
using Nameless.Contracts;

namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Disk-backed <see cref="ITranslationFileProvider"/>. Reads each
///     culture's translation file (<c>{culture}.json</c>, or
///     <c>default.json</c> for the literal <c>"default"</c> key) once per
///     call, from <see cref="TranslatorOptions.RootPath"/>.
/// </summary>
/// <remarks>
///     Read-once-at-startup by design — no caching, no
///     <see cref="System.IO.FileSystemWatcher"/>. <see cref="JsonTranslator"/>
///     already caches the merged dictionary per culture, so re-reading here
///     would only matter for hot-reload, which is a future, watching
///     implementation of this same seam (see ADR-002).
/// </remarks>
public sealed class FileSystemTranslationFileProvider : ITranslationFileProvider {
    private readonly string _rootPath;
    private readonly ILogger<FileSystemTranslationFileProvider> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="FileSystemTranslationFileProvider"/> class.
    /// </summary>
    /// <param name="options">
    ///     The translator options, providing <see cref="TranslatorOptions.RootPath"/>.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public FileSystemTranslationFileProvider(IOptions<TranslatorOptions> options, ILogger<FileSystemTranslationFileProvider> logger) {
        var rootPath = Throws.When.NullOrWhiteSpace(
            Throws.When.Null(options).Value.RootPath,
            exceptionCreator: () => new MissingConfigurationException(
                section: ConfigurationSectionNameAttribute.GetSectionName<TranslatorOptions>(),
                key: nameof(TranslatorOptions.RootPath)
            )
        );

        _rootPath = SysPath.IsPathRooted(rootPath)
            ? rootPath
            : SysPath.Combine(AppContext.BaseDirectory, rootPath);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public bool TryRead(string key, [NotNullWhen(true)] out string? content) {
        var fileName = string.Equals(key, "default", StringComparison.OrdinalIgnoreCase)
            ? "default.json"
            : $"{key}.json";
        var path = SysPath.Combine(_rootPath, fileName);

        if (!SysFile.Exists(path)) {
            content = null;

            return false;
        }

        try {
            content = SysFile.ReadAllText(path);

            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) {
            // A locked or unreadable file should not crash JsonTranslator's
            // indexer for every caller — treat it like a missing file and
            // let the fallback chain (or the next lookup) take over.
            CommonLog.Error(_logger, message: $"Unable to read translation file '{path}'.", exception, tag: key);

            content = null;

            return false;
        }
    }
}
