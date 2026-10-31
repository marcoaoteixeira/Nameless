using Wpf.Ui.Controls;

namespace Nameless.Windows.Notification;

/// <summary>
///     Notification Type Extension Methods
/// </summary>
public static class NotificationTypeExtensions {
    /// <param name="self">
    ///     The current <see cref="NotificationType"/> instance.
    /// </param>
    extension(NotificationType self) {
        /// <summary>
        ///     Converts the current <see cref="NotificationType"/> instance
        ///     to a <see cref="ControlAppearance"/> instance.
        /// </summary>
        /// <returns></returns>
        public ControlAppearance ToControlAppearance() {
            return self switch {
                NotificationType.Failure => ControlAppearance.Danger,
                NotificationType.Success => ControlAppearance.Success,
                NotificationType.Warning => ControlAppearance.Caution,
                _ => ControlAppearance.Primary,
            };
        }
    }
}
