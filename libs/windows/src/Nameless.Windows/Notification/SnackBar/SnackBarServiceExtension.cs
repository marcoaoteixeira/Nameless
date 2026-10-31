using Wpf.Ui;

namespace Nameless.Windows.Notification.SnackBar;

/// <summary>
///     Snack Bar Service Extension Methods
/// </summary>
public static class SnackBarServiceExtension {
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ISnackbarService"/> instance.
    /// </param>
    extension(ISnackbarService self) {
        /// <summary>
        ///     Shows the snack bar notification for the event.
        /// </summary>
        /// <param name="evt">
        ///     The event.
        /// </param>
        public void Show(SnackBarNotificationEvent evt) {
            var appearance = evt.Type.ToControlAppearance();

            self.Show(
                evt.Title ?? appearance.Title,
                evt.Message,
                appearance,
                appearance.Icon,
                Timeout
            );
        }
    }
}