using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;
using Nameless.WPF.UseCases.SystemUpdate.Download;

namespace Nameless.WPF.UseCases.SystemUpdate.Fetch;

internal static class NotificationServiceExtensions {
    extension(IMessenger self) {
        internal Task NotifyStartingAsync() {
            return self.PublishInformationAsync<DownloadUpdateMessage>(
                content: Strings.FetchNewVersionInformationNotification_Starting
            );
        }

        internal Task NotifyFailureAsync(string version, string error) {
            return self.PublishFailureAsync<DownloadUpdateMessage>(
                content: string.Format(Strings.FetchNewVersionInformationNotification_Failure, version, error)
            );
        }

        internal Task NotifyNotFoundAsync() {
            return self.PublishFailureAsync<DownloadUpdateMessage>(
                content: Strings.FetchNewVersionInformationNotification_NotFound
            );
        }

        internal Task NotifySuccessAsync() {
            return self.PublishSuccessAsync<DownloadUpdateMessage>(
                content: Strings.FetchNewVersionInformationNotification_Success
            );
        }
    }
}
