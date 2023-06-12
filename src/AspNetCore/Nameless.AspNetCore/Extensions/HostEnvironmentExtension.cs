using Microsoft.Extensions.Hosting;

namespace Nameless.AspNetCore {

    public static class HostEnvironmentExtension {

        #region Public Constants

        public const string DEVELOPER_MACHINE_ENVIRONMENT_NAME = "DeveloperMachine";

        #endregion

        #region Public Static Methods

        public static bool IsDeveloperMachine(this IHostEnvironment self) {
            Prevent.Null(self, nameof(self));

            return self.IsEnvironment(DEVELOPER_MACHINE_ENVIRONMENT_NAME);
        }

        #endregion
    }
}
