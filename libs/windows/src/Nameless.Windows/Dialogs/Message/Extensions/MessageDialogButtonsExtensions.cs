namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     <see cref="MessageDialogButtons"/> extension methods.
/// </summary>
public static class MessageDialogButtonsExtensions {
    /// <param name="self">
    ///     The current <see cref="MessageDialogButtons"/> instance.
    /// </param>
    extension(MessageDialogButtons self) {
        /// <summary>
        ///     Converts a <see cref="MessageDialogButtons"/> instance into
        ///     a <see cref="SysMessageBoxButton"/> instance.
        /// </summary>
        /// <returns>
        ///     The <see cref="SysMessageBoxButton"/> instance.
        /// </returns>
        public SysMessageBoxButton ToSystem() {
            return self switch {
                MessageDialogButtons.OkCancel => SysMessageBoxButton.OKCancel,
                MessageDialogButtons.YesNo => SysMessageBoxButton.YesNo,
                MessageDialogButtons.YesNoCancel => SysMessageBoxButton.YesNoCancel,
                _ => SysMessageBoxButton.OK,
            };
        }
    }
}