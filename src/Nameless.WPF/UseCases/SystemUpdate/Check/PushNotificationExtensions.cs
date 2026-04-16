using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;

namespace Nameless.WPF.UseCases.SystemUpdate.Check;

internal static class PushNotificationExtensions {
    extension(IMessenger self) {
        internal Task NotifyStartingAsync() {
            return self.PublishInformationAsync<CheckForUpdateMessage>(
                content: Strings.CheckForUpdateRequestHandler_Starting
            );
        }

        internal Task NotifyFailureAsync(string error) {
            return self.PublishFailureAsync<CheckForUpdateMessage>(
                content: error
            );
        }

        internal Task NotifySuccessAsync() {
            return self.PublishSuccessAsync<CheckForUpdateMessage>(
                content: Strings.CheckForUpdateNotification_Success_CurrentVersionUpToDate
            );
        }

        internal Task NotifySuccessAsync(string newVersion) {
            return self.PublishSuccessAsync<CheckForUpdateMessage>(
                content: string.Format(Strings.CheckForUpdateNotification_Success_NewVersionAvailable, newVersion)
            );
        }
    }
}
