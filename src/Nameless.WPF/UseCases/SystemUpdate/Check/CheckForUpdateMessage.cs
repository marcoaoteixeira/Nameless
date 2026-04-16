using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;

namespace Nameless.WPF.UseCases.SystemUpdate.Check;

public record CheckForUpdateMessage : Message {
    public CheckForUpdateMessage() {
        Title = Strings.CheckForUpdateNotification_Title;
    }
}