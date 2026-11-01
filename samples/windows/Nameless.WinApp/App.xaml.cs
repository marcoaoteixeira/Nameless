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
        args: [$"--applicationName={Constants.Application.Name}"],
        assemblies: [typeof(App).Assembly],
        configureFeatures: ApplicationHostFeature.Configure
    ).Wrap();

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

