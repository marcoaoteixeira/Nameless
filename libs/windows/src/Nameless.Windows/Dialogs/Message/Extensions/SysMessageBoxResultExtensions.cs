namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     <see cref="SysMessageBoxResult"/> extension methods.
/// </summary>
public static class SysMessageBoxResultExtensions {
    /// <param name="self">
    ///     The current <see cref="SysMessageBoxResult"/> instance.
    /// </param>
    extension(SysMessageBoxResult self) {
        /// <summary>
        ///     Converts a <see cref="SysMessageBoxResult"/> into a
        ///     <see cref="MessageDialogResult"/> instance.
        /// </summary>
        /// <returns>
        ///     The <see cref="MessageDialogResult"/> instance.
        /// </returns>
        public MessageDialogResult ToMessageDialogResult() {
            return self switch {
                SysMessageBoxResult.Cancel => MessageDialogResult.Cancel,
                SysMessageBoxResult.Yes => MessageDialogResult.Yes,
                SysMessageBoxResult.No => MessageDialogResult.No,
                _ => MessageDialogResult.Ok,
            };
        }
    }
}