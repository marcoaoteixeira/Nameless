using Microsoft.AspNetCore.Http.Timeouts;

namespace Nameless.Web.RequestTimeout;

internal static class RequestTimeoutPolicyOptionsExtensions {
    extension(RequestTimeoutPolicyOptions self) {
        internal RequestTimeoutPolicy ToPolicy() {
            return new RequestTimeoutPolicy {
                Timeout = self.ExpiresIn,
                TimeoutStatusCode = self.HttpStatusCode
            };
        }
    }
}
