using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;

namespace Nameless.Web.RateLimiter;

internal static class RateLimiterOptionsHandlerCache {
    private static readonly Lazy<MethodInfo> LazyHandler;

    internal static MethodInfo Handler => LazyHandler.Value;

    static RateLimiterOptionsHandlerCache() {
        LazyHandler = new Lazy<MethodInfo>();
    }

    private static MethodInfo CreateHandler() {
        return typeof(RateLimiterOptions)
               .GetMethods(BindingFlags.Public | BindingFlags.Instance)
               .Where(method => method.Name.Equals(nameof(RateLimiterOptions.AddPolicy)))
               .Where(method => {
                   var parameters = method.GetParameters();
                   if (parameters.Length != 2) { return false; }

                   // first parameter must be string type
                   if (parameters[0].ParameterType != typeof(string)) { return false; }

                   // second parameter must be IRateLimiterPolicy<T> type
                   var rateLimiterPolicyParam = parameters[1].ParameterType;

                   return rateLimiterPolicyParam.IsGenericType &&
                          rateLimiterPolicyParam.GetGenericTypeDefinition() == typeof(IRateLimiterPolicy<>);
               }).Single(); // not it's unambiguous
    }
}
