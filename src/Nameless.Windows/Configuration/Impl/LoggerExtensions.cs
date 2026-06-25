using Microsoft.Extensions.Logging;

namespace Nameless.Windows.Configuration.Impl;

/// <summary>
///     <see cref="AppConfigurationManager"/> logger extensions.
/// </summary>
internal static class LoggerExtensions {
    private const string TAG = "APP_CONFIGURATION_MANAGER";

    extension(ILogger<AppConfigurationManager> self) {
        internal void TryGetFailure(string key, Type type, Exception exception) {
            Log.AppConfigurationManagerTryGetFailure(
                self,
                key,
                type.FullName ?? type.Name,
                exception
            );
        }

        internal void SaveChangesAsyncFailure(Exception exception) {
            Log.Failure(
                self,
                TAG,
                actionName: nameof(IAppConfigurationManager.SaveChangesAsync),
                exception
            );
        }

        internal void GetAppConfigurationFailure(Exception exception) {
            Log.Failure(
                self,
                TAG,
                actionName: "GetAppConfiguration",
                exception
            );
        }
    }
}
