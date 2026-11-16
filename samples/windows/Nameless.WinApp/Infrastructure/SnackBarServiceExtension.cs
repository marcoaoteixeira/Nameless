using Nameless.WinApp.Infrastructure.Events;
using Nameless.Windows;
using Nameless.Windows.Notification;
using Wpf.Ui;

namespace Nameless.WinApp.Infrastructure;

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
        public void Show(MainWindowNotificationEvent evt) {
            var appearance = evt.Type.ToControlAppearance();

            self.Show(
                evt.Title ?? appearance.ToString(),
                evt.Message,
                appearance,
                appearance.Icon,
                Timeout
            );
        }
    }
}