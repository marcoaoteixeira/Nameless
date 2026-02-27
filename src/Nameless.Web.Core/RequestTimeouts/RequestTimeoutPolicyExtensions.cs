using MS_RequestTimeoutPolicy = Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy;

namespace Nameless.Web.RequestTimeouts;

internal static class RequestTimeoutPolicyExtensions {
    extension(RequestTimeoutPolicy self) {
        internal MS_RequestTimeoutPolicy ToPolicy() {
            return new MS_RequestTimeoutPolicy {
                Timeout = self.ExpiresIn,
                TimeoutStatusCode = self.HttpStatusCode
            };
        }
    }
}
