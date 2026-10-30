using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthCheck.Reporting;

/// <summary>
///     JSON Reporter Writer
/// </summary>
public static class JsonReportWriter {
    private static readonly Lazy<JsonSerializerOptions> Options = new(CreateJsonSerializerOptions);

    /// <summary>
    ///     Writes the health check reporter as JSON.
    /// </summary>
    /// <param name="httpContext">
    ///     The HTTP context.
    /// </param>
    /// <param name="report">
    ///     The health check report.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous method
    ///     execution.
    /// </returns>
    public static async Task WriteAsync(HttpContext httpContext, HealthReport report) {
        httpContext.Response.ContentType = "application/json";

        var result = Report.Create(report);

        await JsonSerializer.SerializeAsync(httpContext.Response.Body,
                                result,
                                Options.Value)
                            .SkipContextSync();
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