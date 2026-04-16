using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Nameless.WPF.Resources;

namespace Nameless.WPF;

public static class ExceptionWarden {
    private static volatile Exception? _last;

    private static string ApplicationName { get; set; } = string.Empty;

    public static void Initialize(string applicationName) {
        ApplicationName = applicationName;

        WindowsApplication.Current.DispatcherUnhandledException += UnhandledExceptionHandler;
        Dispatcher.CurrentDispatcher.UnhandledException += UnhandledExceptionHandler;
        TaskScheduler.UnobservedTaskException += UnhandledExceptionHandler;
    }

    private static void UnhandledExceptionHandler(object? _, UnobservedTaskExceptionEventArgs args) {
        args.SetObserved();

        ExceptionHandler(args.Exception);
    }

    private static void UnhandledExceptionHandler(object _, DispatcherUnhandledExceptionEventArgs args) {
        args.Handled = true;

        ExceptionHandler(args.Exception);
    }

    private static void ExceptionHandler(Exception exception) {
        if (_last is not null) { return; }

        _last = exception;

        var messageBoxResult = SysMessageBox.Show(
            exception.Message,
            Strings.ExceptionWarden_MessageBox_Title,
            SysMessageBoxButton.OK,
            SysMessageBoxImage.Error
        );

        if (messageBoxResult == SysMessageBoxResult.OK) {
            Seppuku();
        }
    }

    private static void Seppuku() {
        using var self = Process.GetProcessesByName(ApplicationName)
                                .SingleOrDefault();

        self?.Kill();
    }
}
