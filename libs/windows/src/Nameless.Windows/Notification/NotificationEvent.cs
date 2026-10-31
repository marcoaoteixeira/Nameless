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
    public string? Title { get; init; }

    /// <summary>
    ///     Gets or sets the message.
    /// </summary>
    public string Message {
        get;
        init => field = Throws.When.Null(value);
    } = string.Empty;

    /// <summary>
    ///     Gets or sets the type.
    /// </summary>
    public NotificationType Type { get; init; }

    /// <summary>
    ///     Gets
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
}