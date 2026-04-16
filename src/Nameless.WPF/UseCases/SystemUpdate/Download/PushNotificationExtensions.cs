using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

internal static class PushNotificationExtensions {
    extension(IMessenger self) {
        internal Task NotifyStartingAsync() {
            return self.PublishInformationAsync<DownloadUpdateMessage>(
                content: Strings.DownloadUpdateNotification_Starting
            );
        }

        internal Task NotifyWritingFileAsync() {
            return self.PublishInformationAsync<DownloadUpdateMessage>(
                content: Strings.DownloadUpdateNotification_WritingFile
            );
        }

        internal Task NotifySuccessAsync(string filePath) {
            return self.PublishSuccessAsync<DownloadUpdateMessage>(
                content: string.Format(Strings.DownloadUpdateNotification_Success, filePath)
            );
        }
        internal Task NotifyFailureAsync(string error) {
            return self.PublishFailureAsync<DownloadUpdateMessage>(
                content: error
            );
        }
    }
}
