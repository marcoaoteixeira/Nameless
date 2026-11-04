using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.WinApp.Infrastructure;
using Nameless.WinApp.Views.Windows;
using Nameless.Windows;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Hosting;
using Nameless.Windows.Hosting.Wrappers;
using Nameless.Windows.Localization;
using Wpf.Ui;

namespace Nameless.WinApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App {
    private static WinHost CurrentHost { get; } = WinHostFactory.CreateDefault(
        args: BuildHostArgs(),
        assemblies: [typeof(App).Assembly],
        configureFeatures: ApplicationHostFeature.Configure
    ).Wrap();

    // IApplicationContext.Version must come from the same assembly-stamped
    // informational version the self-update feature compares against
    // release tags (ADR-001 / release contract §3) — not from
    // AssemblyVersion, which stays pinned to 1.0.0 repo-wide.
    private static string[] BuildHostArgs() {
        var args = new List<string> { $"--applicationName={Constants.Application.Name}" };

        var version = typeof(App).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(version)) {
            args.Add($"--ApplicationContext:Version={version}");
        }

        return [.. args];
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="App"/> class.
    /// </summary>
    public App() {
        ExceptionWarden.Initialize(Constants.Application.Name);

        CurrentHost.OnStart += OnCurrentHostStart;
        CurrentHost.OnStop += OnCurrentHostStop;
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnStartup(StartupEventArgs e) {
        await CurrentHost.RunAsync();

        base.OnStartup(e);
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnExit(ExitEventArgs e) {
        await CurrentHost.StopAsync();

        CurrentHost.Dispose();

        base.OnExit(e);
    }

    private static Task OnCurrentHostStart(IServiceProvider provider, CancellationToken cancellationToken) {
        TranslatorExtension.SetTranslator(
            provider.GetService<ITranslator>() ?? NullTranslator.Instance
        );

        var main = provider.GetRequiredService<INavigationWindow>();

        provider.GetRequiredService<ISplashScreenWindow>()
                .Show(WindowStartupLocation.CenterScreen);

        main.ShowWindow();

        return Task.CompletedTask;
    }

    private static async Task OnCurrentHostStop(IServiceProvider provider, CancellationToken cancellationToken) {
        await provider.GetRequiredService<IAppConfigurationManager>()
                      .SaveChangesAsync(cancellationToken)
                      .SkipContextSync();
    }
}

