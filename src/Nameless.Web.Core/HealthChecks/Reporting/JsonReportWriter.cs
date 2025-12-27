using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthChecks.Reporting;

public static class JsonReportWriter {
    private static readonly Lazy<JsonSerializerOptions> Options = new(CreateJsonSerializerOptions);

    public static async Task WriteAsync(HttpContext httpContext, HealthReport report) {
        httpContext.Response.ContentType = "application/json";

        var result = Report.Create(report);

        await JsonSerializer.SerializeAsync(httpContext.Response.Body,
                                result,
                                Options.Value)
                            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions() {
        var options = new JsonSerializerOptions {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}