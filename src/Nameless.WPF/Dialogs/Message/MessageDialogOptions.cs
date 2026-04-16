using System.Windows;

namespace Nameless.WPF.Dialogs.Message;

public class MessageDialogOptions {
    public string? Title { get; set; }

    public MessageDialogButtons Buttons { get; set; }

    public MessageDialogIcon Icon { get; set; }

    public Window? Owner { get; set; }
}