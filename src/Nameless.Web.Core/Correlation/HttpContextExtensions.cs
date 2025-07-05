using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Correlation;

internal static class HttpContextExtensions {
    internal static bool HasCorrelationID(this HttpContext self, string key, bool useHeader = true) {
        Prevent.Argument.NullOrWhiteSpace(key);

        return useHeader
            ? GetFromHttpContextHeaders(self, key) is not null
            : GetFromHttpContextItems(self, key) is not null;
    }

    internal static string? GetCorrelationID(this HttpContext self, string key, bool useHeader = true) {
        Prevent.Argument.NullOrWhiteSpace(key);

        return useHeader
            ? GetFromHttpContextHeaders(self, key)
            : GetFromHttpContextItems(self, key);
    }

    internal static void SetCorrelationID(this HttpContext self, string value, string key, bool useHeader = true) {
        Prevent.Argument.NullOrWhiteSpace(key);
        Prevent.Argument.NullOrWhiteSpace(value);

        if (useHeader) { self.Response.Headers[key] = value; }
        else { self.Items[key] = value; }
    }

    private static string? GetFromHttpContextItems(HttpContext context, string key) {
        string? result = null;

        if (context.Items.TryGetValue(key, out var value)) {
            result = string.IsNullOrWhiteSpace(value?.ToString()) ? null : value.ToString();
        }

        return result;
    }

    private static string? GetFromHttpContextHeaders(HttpContext context, string key) {
        string? result = null;

        if (context.Request.Headers.TryGetValue(key, out var value) ||
            context.Response.Headers.TryGetValue(key, out value)) {
            result = string.IsNullOrWhiteSpace(value) ? null : value.ToString();
        }

        return result;
    }
}
