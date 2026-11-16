using Nameless.Windows.Notification;

namespace Nameless.WinApp.Infrastructure.Events;

/// <summary>
///     Main Window Notification Event
/// </summary>
public abstract record MainWindowNotificationEvent : NotificationEvent {
    public NotificationMode Mode { get; set; }

    protected MainWindowNotificationEvent(NotificationMode mode = NotificationMode.None) {
        Mode = mode;
    }
}

public enum NotificationMode {
    None,

    SnackBar,

    MessageDialog,
}