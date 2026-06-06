using System.Windows;
using Microsoft.Extensions.Hosting;
using Nameless.WPF;
using Nameless.WPF.Hosting;

namespace Nameless.WinApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App {
    private static readonly string[] Args = [
        $"--applicationName={Constants.Application.Name}"
    ];

    private static readonly WinHost CurrentHost = WinHostFactory.Create(
        configure => configure.Args = Args
    );

    public App() {
        ExceptionWarden.Initialize(Constants.Application.Name);
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
}