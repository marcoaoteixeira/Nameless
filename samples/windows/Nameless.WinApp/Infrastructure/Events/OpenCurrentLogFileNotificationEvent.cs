namespace Nameless.WinApp.Infrastructure.Events;

public sealed record OpenCurrentLogFileNotificationEvent() : MainWindowNotificationEvent(mode: NotificationMode.SnackBar);