using Microsoft.AspNetCore.OutputCaching;

namespace Nameless.Web.OutputCache;

public static class OutputCachePolicyOptionsExtensions {
    extension(OutputCachePolicyOptions self) {
        public void ToPolicy(OutputCachePolicyBuilder builder) {
            builder.Expire(self.Duration);
        }
    }
}