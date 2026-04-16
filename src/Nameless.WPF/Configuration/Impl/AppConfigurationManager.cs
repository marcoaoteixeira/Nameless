using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Application;
using Nameless.IO.FileSystem;

namespace Nameless.WPF.Configuration.Impl;

/// <summary>
///     Default implementation of <see cref="IAppConfigurationManager"/>.
/// </summary>
public class AppConfigurationManager : IAppConfigurationManager {
    private const string APP_CONFIGURATION_FILE = "app.config";

    private readonly IApplicationContext _applicationContext;
    private readonly ILogger<AppConfigurationManager> _logger;
    private readonly Lazy<Dictionary<string, JsonElement>> _appConfiguration;

    private Dictionary<string, JsonElement> AppConfiguration => _appConfiguration.Value;

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
        _appConfiguration = new Lazy<Dictionary<string, JsonElement>>(GetAppConfiguration);
    }

    /// <inheritdoc />
    public bool TryGet<TValue>(string name, [NotNullWhen(returnValue: true)] out TValue? output) {
        output = default;

        var element = AppConfiguration.GetValueOrDefault(name);
        if (element.ValueKind == JsonValueKind.Undefined) {
            return false;
        }

        try {
            var value = element.Deserialize<TValue>();
            if (value is not null) {
                output = value;

                return true;
            }
        }
        catch (Exception ex) { _logger.TryGetValueFailure(name, typeof(TValue), ex); }

        return false;
    }

    /// <inheritdoc />
    public void Set<TValue>(string name, TValue value) {
        AppConfiguration[name] = JsonSerializer.SerializeToElement(value);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken) {
        try {
            var file = _applicationContext.FileSystemProvider.GetFile(APP_CONFIGURATION_FILE);
            var json = JsonSerializer.SerializeToUtf8Bytes(AppConfiguration);

            await using var stream = file.Open(FileMode.Create);
            await stream.WriteAsync(json, cancellationToken);
        }
        catch (Exception ex) { _logger.SaveAppConfigurationFailure(ex); }
    }

    private Dictionary<string, JsonElement> GetAppConfiguration() {
        var file = _applicationContext.FileSystemProvider.GetFile(APP_CONFIGURATION_FILE);

        if (!file.Exists) { return []; }

        using var stream = file.Open();

        try { return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream) ?? []; }
        catch (Exception ex) { _logger.LoadingConfigurationFailure(ex); }

        return [];
    }
}