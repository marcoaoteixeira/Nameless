using System.Windows;
using Microsoft.Extensions.Hosting;
using Nameless.WPF.Hosting;

namespace Nameless.WinApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application {
    private static readonly string[] Args = [
        $"--applicationName={Constants.ApplicationName}"
    ];

    private static readonly IHost CurrentHost = WinHostFactory.Create(
        configure => configure.Args = Args
    );

    protected override async void OnStartup(StartupEventArgs e) {
        await CurrentHost.RunAsync();

        base.OnStartup(e);
    }
}

internal static class Constants {
    internal const string ApplicationName = "App";
}