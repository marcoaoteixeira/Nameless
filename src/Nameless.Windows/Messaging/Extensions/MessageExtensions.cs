using Nameless.Windows.SnackBar;

namespace Nameless.Windows.Messaging;

public static class MessageExtensions {
    extension(Message self) {
        public SnackBarArgs ToSnackBarArgs() {
            return new SnackBarArgs {
                Title = self.Title,
                Content = self.Content,
                Appearance = self.Type.ToControlAppearance()
            };
        }
    }
}
