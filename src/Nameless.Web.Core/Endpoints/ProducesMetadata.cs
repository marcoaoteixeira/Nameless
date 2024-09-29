using System.Net;

namespace Nameless.Web.Endpoints;

public sealed record ProducesMetadata {
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
    public Type ResponseType { get; init; } = typeof(void);
    public string ContentType { get; init; } = string.Empty;
    public string[] AdditionalContentTypes { get; init; } = [];
    public ProducesResultType Type { get; init; }
}