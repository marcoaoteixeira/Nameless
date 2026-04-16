namespace Nameless.WPF.Dialogs.Message;

/// <summary>
///     Represents which buttons the user should be presented.
/// </summary>
public enum MessageDialogButtons {
    /// <summary>
    ///    Button for acknowledge.
    /// </summary>
    Ok,

    /// <summary>
    ///     Buttons for acknowledge or cancel.
    /// </summary>
    OkCancel,

    /// <summary>
    ///     Buttons for accept or deny.
    /// </summary>
    YesNo,

    /// <summary>
    ///     Buttons for accept, deny or cancel.
    /// </summary>
    YesNoCancel,
}