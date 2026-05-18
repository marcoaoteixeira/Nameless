namespace Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

internal record EndpointMetadata(
    string HttpVerb,
    string Route,
    string? Group
) {
    internal static EndpointMetadata Default => new(HttpVerbs.GET, Route: string.Empty, Group: null);
}