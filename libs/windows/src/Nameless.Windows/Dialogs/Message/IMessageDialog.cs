namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog Contract
/// </summary>
public interface IMessageDialog {
    /// <summary>
    ///     Shows a message dialog in the UI.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="configure">
    ///     The dialog configuration delegate.
    /// </param>
    /// <returns>
    ///     A <see cref="MessageDialogResult"/> instance.
    /// </returns>
    MessageDialogResult Show(string message, Action<MessageDialogOptions> configure);
}