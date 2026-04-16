namespace Nameless.WPF.Dialogs.Message;

/// <summary>
///     Represents icons to be shown in the user dialog message.
/// </summary>
public enum MessageDialogIcon {
    /// <summary>
    ///     An information dialog.
    /// </summary>
    Information,

    /// <summary>
    ///     The dialog means that the action should be
    ///     acknowledged by the user.
    /// </summary>
    Warning,

    /// <summary>
    ///     The dialog means that an error occurred.
    /// </summary>
    Error,

    /// <summary>
    ///     The dialog requires the user attention for the
    ///     action being performed.
    /// </summary>
    Attention,

    /// <summary>
    ///     The dialog requires that the user takes a decision before
    ///     the action can be performed.
    /// </summary>
    Question,
}