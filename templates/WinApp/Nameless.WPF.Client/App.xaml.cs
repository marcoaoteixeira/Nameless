using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.WPF.Client.Views.Windows;
using Nameless.WPF.Configuration;
using Wpf.Ui;

namespace Nameless.WPF.Client;

/// <summary>
///     Entry point
/// </summary>
public partial class App {
    private static readonly Assembly[] SupportAssemblies = [
        typeof(ClientAssemblyMarker).Assembly,
        typeof(CoreAssemblyMarker).Assembly,
        typeof(SqliteAssemblyMarker).Assembly,
        typeof(LuceneAssemblyMarker).Assembly
    ];

    private static readonly string[] Args = [
        $"--applicationName={Constants.Application.Name}"
    ];

    private static readonly IHost CurrentHost = HostFactory.Create(Args)
                                                           .ConfigureServices(ConfigureServices)
                                                           .OnStartup(OnHostStartup)
                                                           .OnTearDown(OnHostTearDown)
                                                           .Build();

    public App() { ExceptionWarden.Initialize(Constants.Application.Name); }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnStartup(StartupEventArgs e) {
        await CurrentHost.StartAsync()
                         .SkipContextSync();

        base.OnStartup(e);
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnExit(ExitEventArgs e) {
        await CurrentHost.StopAsync()
                         .SkipContextSync();

        CurrentHost.Dispose();

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        services.RegisterWPF(SupportAssemblies, configuration);
    }

    private static void OnHostStartup(IServiceProvider provider) {
        var main = provider.GetRequiredService<INavigationWindow>();

        provider.GetRequiredService<ISplashScreenWindow>()
                .Show(WindowStartupLocation.CenterScreen);

        main.ShowWindow();
    }

    // ReSharper disable once AsyncVoidMethod
    private static async void OnHostTearDown(IServiceProvider provider) {
        await provider.GetRequiredService<IAppConfigurationManager>()
                      .SaveChangesAsync(CancellationToken.None)
                      .SkipContextSync();
    }
}
