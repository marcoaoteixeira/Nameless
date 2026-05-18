using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.WPF.Hosting.Configs;
using Nameless.WPF.Hosting.Internals;

namespace Nameless.WPF.Hosting;

public sealed class WinHostFactory {
    private WinHostSettings Settings { get; }

    private WinHostFactory(WinHostSettings settings) {
        Settings = settings;
    }

    public static WinHost Create(Action<WinHostSettings>? configure = null) {
        return new WinHostFactory(ActionHelper.FromDelegate(configure)).CreateBuilder()
                                                                       .Build()
                                                                       .Wrap();
    }

    private WinHostBuilder CreateBuilder() {
        var env = string.IsNullOrWhiteSpace(Settings.Environment)
            ? "Development"
            : Settings.Environment;

        return Host.CreateDefaultBuilder(Settings.Args)
                   .ConfigureHostConfiguration(config => {
                       config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                             .AddJsonFile(path: "AppSettings.json", optional: true, reloadOnChange: true)
                             .AddJsonFile(path: $"AppSettings.{env}.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables();
                   })

                   .Wrap()

                   .ConfigureAppConfigurationManager(Settings)
                   .ConfigureBootstrap(Settings)
                   .ConfigureCommonServices()
                   .ConfigureCompressorServices(Settings)
                   .ConfigureContentDialogService(Settings)
                   .ConfigureDocumentServices(Settings)
                   .ConfigureFileSystemDialog(Settings)
                   .ConfigureFileSystemProvider(Settings)
                   .ConfigureGitHubHttpClient(Settings)
                   .ConfigureHttpClientDefaults(Settings)
                   .ConfigureLogging(Settings)
                   .ConfigureLucene(Settings)
                   .ConfigureMediator(Settings)
                   .ConfigureMessageDialog(Settings)
                   .ConfigureMessenger(Settings)
                   .ConfigureNavigation(Settings)
                   .ConfigureOfficeServices(Settings)
                   .ConfigureResilience(Settings)
                   .ConfigureSnackBar(Settings)
                   .ConfigureTaskRunner(Settings)
                   .ConfigureValidation(Settings)
                   .ConfigureViewModels(Settings)
                   .ConfigureWindowFactory(Settings)

                   // Intentionally placed last so it can override
                   // other services.
                   .ConfigureAdditionalServices(Settings);
    }
}