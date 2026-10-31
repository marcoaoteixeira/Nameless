namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog Icon Extension Methods
/// </summary>
public static class MessageDialogIconExtensions {
    /// <param name="self">
    ///     The current <see cref="MessageDialogIcon"/> instance.
    /// </param>
    extension(MessageDialogIcon self) {
        /// <summary>
        ///     Converts the <see cref="MessageDialogIcon"/> instance into
        ///     a <see cref="SysMessageBoxImage"/> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="SysMessageBoxImage"/> instance
        /// </returns>
        public SysMessageBoxImage ToSystem() {
            return self switch {
                MessageDialogIcon.Warning => SysMessageBoxImage.Warning,
                MessageDialogIcon.Error => SysMessageBoxImage.Error,
                MessageDialogIcon.Attention => SysMessageBoxImage.Exclamation,
                MessageDialogIcon.Question => SysMessageBoxImage.Question,
                _ => SysMessageBoxImage.Information,
            };
        }
    }
}