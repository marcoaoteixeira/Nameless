using Microsoft.AspNetCore.OutputCaching;

namespace Nameless.Web.OutputCache;

/// <summary>
///     Output Cache Policy Options Extensions.
/// </summary>
internal static class OutputCachePolicyOptionsExtensions {
    extension(OutputCachePolicyOptions self) {
        /// <summary>
        ///     Sets the policy duration.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="OutputCachePolicyBuilder"/> instance.
        /// </param>
        public void SetDuration(OutputCachePolicyBuilder builder) {
            builder.Expire(self.Duration);
        }
    }
}