using Wpf.Ui.Controls;

namespace Nameless.WPF.Messaging;

public static class MessageTypeExtensions {
    extension(MessageType self) {
        public ControlAppearance ToControlAppearance() {
            return self switch {
                MessageType.Failure => ControlAppearance.Danger,
                MessageType.Success => ControlAppearance.Success,
                MessageType.Warning => ControlAppearance.Caution,
                _ => ControlAppearance.Primary,
            };
        }
    }
}
