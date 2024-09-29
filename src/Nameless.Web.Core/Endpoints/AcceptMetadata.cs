namespace Nameless.Web.Endpoints;
public sealed record AcceptMetadata {
    public Type? RequestType { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string[] AdditionalContentTypes { get; init; } = [];
}
