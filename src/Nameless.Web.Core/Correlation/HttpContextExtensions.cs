using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Correlation;

public static class HttpContextExtensions {
    private const string CORRELATION_ID_KEY = "X-Correlation-ID";

    extension(HttpContext self) {
        public bool HasCorrelationID(string? key = null, bool useHeader = true) {
            key ??= CORRELATION_ID_KEY;

            Throws.When.NullOrWhiteSpace(key);

            return useHeader
                ? GetFromHttpContextHeaders(self, key) is not null
                : GetFromHttpContextItems(self, key) is not null;
        }

        public string? GetCorrelationID(string? key = null, bool useHeader = true) {
            key ??= CORRELATION_ID_KEY;

            Throws.When.NullOrWhiteSpace(key);

            return useHeader
                ? GetFromHttpContextHeaders(self, key)
                : GetFromHttpContextItems(self, key);
        }

        public void SetCorrelationID(string value, string? key = null, bool useHeader = true) {
            key ??= CORRELATION_ID_KEY;

            Throws.When.NullOrWhiteSpace(key);
            Throws.When.NullOrWhiteSpace(value);

            if (useHeader) { self.Response.Headers[key] = value; }
            else { self.Items[key] = value; }
        }
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