using Nameless.WPF.Resources;

namespace Nameless.WPF.Dialogs.Message.Extensions;

public static class MessageDialogIconExtensions {
    extension(MessageDialogIcon self) {
        public SysMessageBoxImage ToSystem() {
            return self switch {
                MessageDialogIcon.Warning => SysMessageBoxImage.Warning,
                MessageDialogIcon.Error => SysMessageBoxImage.Error,
                MessageDialogIcon.Attention => SysMessageBoxImage.Exclamation,
                MessageDialogIcon.Question => SysMessageBoxImage.Question,
                _ => SysMessageBoxImage.Information,
            };
        }

        public string AlternativeText {
            get => self switch {
                MessageDialogIcon.Warning => Strings.MessageDialogIcon_Warning,
                MessageDialogIcon.Error => Strings.MessageDialogIcon_Error,
                MessageDialogIcon.Question => Strings.MessageDialogIcon_Question,
                MessageDialogIcon.Attention => Strings.MessageDialogIcon_Attention,
                _ => Strings.MessageDialogIcon_Information,
            };
        }
    }
}