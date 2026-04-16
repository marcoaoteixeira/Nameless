using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

public record DownloadUpdateMessage : Message {
    public DownloadUpdateMessage() {
        Title = Strings.DownloadUpdateNotification_Title;
    }
}
