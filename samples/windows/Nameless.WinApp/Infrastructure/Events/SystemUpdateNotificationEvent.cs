namespace Nameless.WinApp.Infrastructure.Events;

public sealed record SystemUpdateNotificationEvent() : MainWindowNotificationEvent(mode: NotificationMode.None);