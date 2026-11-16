using Nameless.Contracts;
using Nameless.Windows.Events;

namespace Nameless.Windows.Notification;

/// <summary>
/// 
/// </summary>
public abstract record NotificationEvent : Event {
    /// <summary>
    ///     Gets or sets the title.
    /// </summary>
    public string Title {
        get => string.IsNullOrWhiteSpace(field) ? GetType().Name : field;
        set;
    }

    /// <summary>
    ///     Gets or sets the message.
    /// </summary>
    public string Message {
        get;
        set => field = Throws.When.Null(value);
    } = string.Empty;

    /// <summary>
    ///     Gets or sets the type.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    ///     Gets or sets the notification metadata.
    /// </summary>
    public Dictionary<string, object?>? Metadata { get; set; }
}