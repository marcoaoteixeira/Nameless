using System.Text;

namespace Nameless.Web.Http.Endpoints.Infrastructure;

internal static class SourceCodeHelper {
    public static readonly string Namespace = "Nameless.Web.Http.Endpoint.TestUseCases";

    public static string Write(string code) {
        var sb = new StringBuilder();

        WriteUsingBlock(sb);
        WriteNamespace(sb);

        sb.Append(code);

        return sb.ToString();
    }

    private static void WriteUsingBlock(StringBuilder sb) {
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Antiforgery;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Authorization;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.CookieRedirect;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Cors;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Filtering;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.HttpMetrics;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.OutputCache;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Produces;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.RateLimiting;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.RequestTimeout;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Validation;");
        sb.AppendLine("using global::Nameless.Web.Http.Endpoints.Attributes.Versioning;");

        sb.AppendLine("using AllowCookieRedirectAttribute = global::Nameless.Web.Http.Endpoints.Attributes.CookieRedirect.AllowCookieRedirectAttribute;");
        sb.AppendLine("using DisableHttpMetricsAttribute = global::Nameless.Web.Http.Endpoints.Attributes.HttpMetrics.DisableHttpMetricsAttribute;");
        sb.AppendLine("using DeprecateAttribute = global::Nameless.Web.Http.Endpoints.Attributes.Versioning.DeprecateAttribute;");

        sb.AppendLine("using Microsoft.AspNetCore.Http;");

        sb.AppendLine();
    }

    private static void WriteNamespace(StringBuilder sb) {
        sb.AppendLine($"namespace {Namespace};");
        sb.AppendLine();
    }
}
