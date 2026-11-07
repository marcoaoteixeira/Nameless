using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Application;
using Nameless.IO;

namespace Nameless.Windows.AppConfiguration;

/// <summary>
///     Default implementation of <see cref="IAppConfigurationManager"/>.
/// </summary>
public class AppConfigurationManager : IAppConfigurationManager {
    private const string APP_CONFIGURATION_FILE = "app.config";

    private readonly IApplicationContext _applicationContext;
    private readonly ILogger<AppConfigurationManager> _logger;
    private Dictionary<string, JsonElement>? _appConfiguration;

    private Dictionary<string, JsonElement> AppConfiguration => _appConfiguration ??= GetAppConfiguration();

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AppConfigurationManager"/> class.
    /// </summary>
    /// <param name="applicationContext">
    ///     The application context.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public AppConfigurationManager(IApplicationContext applicationContext, ILogger<AppConfigurationManager> logger) {
        _applicationContext = applicationContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool TryGet<TValue>(string name, [NotNullWhen(returnValue: true)] out TValue? output) {
        output = default;

        var element = AppConfiguration.GetValueOrDefault(name);
        if (element.ValueKind == JsonValueKind.Undefined) { return false; }

        try {
            output = element.Deserialize<TValue>();

            return output is not null;
        }
        catch (Exception ex) { CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag); }

        return false;
    }

    /// <inheritdoc />
    public void Set<TValue>(string name, TValue value) {
        AppConfiguration[name] = JsonSerializer.SerializeToElement(value);
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken) {
        var file = _applicationContext.FileManager.GetFile(APP_CONFIGURATION_FILE);

        // Write-then-rename: a concurrent reader (e.g. a freshly relaunched
        // process racing the old process's shutdown-time save) must never
        // observe a truncated/partial app.config. The temp file lives next
        // to the destination so the final move is an atomic rename on the
        // same volume, not a copy. A single fixed name (rather than a
        // per-call guid) means at most one orphan can ever exist: the next
        // save's FileMode.Create overwrites it.
        var tempFile = _applicationContext.FileManager.GetFile($"{APP_CONFIGURATION_FILE}.tmp");
        var tempCreated = false;

        try {
            cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.SerializeToUtf8Bytes(AppConfiguration);

            using (var stream = tempFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
                tempCreated = true;

                stream.Write(json);
            }

            cancellationToken.ThrowIfCancellationRequested();

            SysFile.Move(tempFile.Path, file.Path, overwrite: true);
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag);

            // Only delete the temp file if this call actually created/opened
            // it. With a fixed name, a failure to open it (e.g. another
            // writer already holds it with FileShare.None) must not delete
            // that other writer's in-flight temp file.
            if (tempCreated && tempFile.Exists) {
                try { tempFile.Delete(); }
                catch (IOException) { /* best effort cleanup */ }
                catch (UnauthorizedAccessException) { /* best effort cleanup */ }
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Reload() {
        _appConfiguration = GetAppConfiguration();
    }

    private Dictionary<string, JsonElement> GetAppConfiguration() {
        var file = _applicationContext.FileManager.GetFile(APP_CONFIGURATION_FILE);

        if (!file.Exists) { return []; }

        using var stream = file.Open();

        try { return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream) ?? []; }
        catch (Exception ex) { CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag); }

        return [];
    }
}