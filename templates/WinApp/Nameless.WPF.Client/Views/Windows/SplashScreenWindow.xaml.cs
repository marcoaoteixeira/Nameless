using System.Windows;
using Microsoft.Extensions.Configuration;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;
using Nameless.Infrastructure;
using Nameless.WPF.Windows;

namespace Nameless.WPF.Client.Views.Windows;

public partial class SplashScreenWindow : ISplashScreenWindow {
    private readonly IApplicationContext _applicationContext;
    private readonly IBootstrapper _bootstrapper;
    private readonly IConfiguration _configuration;
    private readonly IProgress<StepProgress> _progress;

    public SplashScreenWindow(
        IApplicationContext applicationContext,
        IBootstrapper bootstrapper,
        IConfiguration configuration) {
        _applicationContext = applicationContext;
        _bootstrapper = bootstrapper;
        _configuration = configuration;

        _progress = new Progress<StepProgress>(UpdateControls);

        InitializeComponent();
        Initialize();
    }

    private void Initialize() {
        ApplicationVersionTextBlock.Text = _applicationContext.Version;
    }

    public void Show(WindowStartupLocation startupLocation) {
        WindowStartupLocation = startupLocation;

        ShowDialog();
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void SplashScreenReady(object? sender, EventArgs e) {
        var timeout = GetBootstrapTimeout();
        using var cts = new CancellationTokenSource(timeout);

        await _bootstrapper.ExecuteAsync(context: [], _progress, cts.Token)
                           .ContinueWith(_ => Dispatcher.Invoke(Close), cts.Token)
                           .SkipContextSync();
    }

    private void UpdateControls(StepProgress report) {
        Dispatcher.Invoke(() => {
            StepNameTextBlock.Text = report.StepName;
            StepMessageTextBlock.Text = report.Message;
        });
    }

    private int GetBootstrapTimeout() {
        var value = _configuration.GetSection(nameof(Bootstrapper))
                                  .Get<int>();

        return value > 0 ? value : -1;
    }
}

public interface ISplashScreenWindow : IWindow;