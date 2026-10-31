using System.Windows;

namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog Options
/// </summary>
public class MessageDialogOptions {
    /// <summary>
    ///     Gets or sets the message dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or sets the message dialog buttons.
    /// </summary>
    public MessageDialogButtons Buttons { get; set; }

    /// <summary>
    ///     Gets or sets the message dialog icon.
    /// </summary>
    public MessageDialogIcon Icon { get; set; }

    /// <summary>
    ///     Gets or sets windows that owns the message dialog.
    /// </summary>
    public Window? Owner { get; set; }
}