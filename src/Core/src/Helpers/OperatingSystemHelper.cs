using System.Runtime.InteropServices;

namespace Nameless.Helpers {
    public static class OperatingSystemHelper {
        #region Public Static Properties

        public static bool IsWindows => RuntimeInformation.IsOSPlatform (OSPlatform.Windows);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform (OSPlatform.Linux);
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform (OSPlatform.OSX);

        #endregion
    }
}