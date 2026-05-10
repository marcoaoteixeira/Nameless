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
        var factory = new WinHostFactory(
            ActionHelper.FromDelegate(configure)
        );

        var host = factory.CreateBuilder()
                          .Build();

        return new WinHost(host);
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

                   .RegisterAppConfigurationManager(Settings)
                   .RegisterBootstrap(Settings)
                   .RegisterCommon()
                   .RegisterCompressor(Settings)
                   .RegisterContentDialogService(Settings)
                   .RegisterDocumentServices(Settings)
                   .RegisterFileSystemDialog(Settings)
                   .RegisterFileSystemProvider(Settings)
                   .RegisterGitHubHttpClient(Settings)
                   .RegisterHttpClientDefaults(Settings)
                   .RegisterLogging(Settings)
                   .RegisterLucene(Settings)
                   .RegisterMediator(Settings)
                   .RegisterMessageDialog(Settings)
                   .RegisterMessenger(Settings)
                   .RegisterNavigation(Settings)
                   .RegisterOffice(Settings)
                   .RegisterResilience(Settings)
                   .RegisterSnackBar(Settings)
                   .RegisterTaskRunner(Settings)
                   .RegisterValidation(Settings)
                   .RegisterViewModels(Settings)
                   .RegisterWindowFactory(Settings)

                   // Intentionally placed last so it can override other services.
                   .RegisterAdditionalConfigurations(Settings);
    }
}