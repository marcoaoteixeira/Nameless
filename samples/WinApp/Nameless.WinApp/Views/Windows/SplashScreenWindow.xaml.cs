using System.Windows;
using Microsoft.Extensions.Configuration;
using Nameless.Application;
using Nameless.Bootstrap;
using Nameless.Reporting;
using Nameless.Windows.UI;

namespace Nameless.WinApp.Views.Windows;

public partial class SplashScreenWindow : ISplashScreenWindow {
    private readonly IApplicationContext _applicationContext;
    private readonly IBootstrapper _bootstrapper;
    private readonly IConfiguration _configuration;
    private readonly IStatusMonitor<Bootstrapper> _bootstrapperStatusMonitor;

    public SplashScreenWindow(
        IApplicationContext applicationContext,
        IBootstrapper bootstrapper,
        IConfiguration configuration,
        IStatusMonitor<Bootstrapper> bootstrapperStatusMonitor) {
        _applicationContext = applicationContext;
        _bootstrapper = bootstrapper;
        _configuration = configuration;
        _bootstrapperStatusMonitor = bootstrapperStatusMonitor;

        InitializeComponent();
        Initialize();
    }

    private void Initialize() {
        ApplicationVersionTextBlock.Text = _applicationContext.Version;

        _bootstrapperStatusMonitor.Status.Subscribe(
            onNext: UpdateControls
        );
    }

    public void Show(WindowStartupLocation startupLocation) {
        WindowStartupLocation = startupLocation;

        ShowDialog();
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void SplashScreenReady(object? sender, EventArgs e) {
        var timeout = GetBootstrapTimeout();
        using var cts = new CancellationTokenSource(timeout);

        await _bootstrapper.RunAsync(cts.Token)
                           .ContinueWith(_ => Dispatcher.Invoke(Close), cts.Token)
                           .SkipContextSync();
    }

    private void UpdateControls(StatusUpdate report) {
        Dispatcher.Invoke(() => {
            StepNameTextBlock.Text = string.Empty;
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