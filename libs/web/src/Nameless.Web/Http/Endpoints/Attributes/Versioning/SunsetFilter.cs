using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

/// <summary>
///     Filter responsible to include the sunset information about the
///     endpoint into the response header.
/// </summary>
public class SunsetFilter : IEndpointFilter {
    /// <inheritdoc />
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        WriteResponseSunsetInformation(context.HttpContext);

        return next(context);
    }

    private static void WriteResponseSunsetInformation(HttpContext context) {
        var sunset = context.GetEndpoint()?.Metadata.GetMetadata<SunsetMetadata>();
        if (sunset is null) { return; }

        context.Response.Headers.Append("Sunset", sunset.SunsetDate.ToString("R"));

        if (sunset.Link is not null) {
            context.Response.Headers.Append("Link", $"<{sunset.Link}>; rel=\"sunset\"");
        }
    }
}