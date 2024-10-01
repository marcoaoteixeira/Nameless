using Microsoft.AspNetCore.Authorization;

namespace Nameless.Web.Endpoints;

public abstract class EndpointBase {
    public abstract string HttpMethod { get; }

    public abstract string RoutePattern { get; }

    public abstract Delegate CreateDelegate();

    public virtual IEnumerable<Accept> GetAccepts()
        => [];

    public virtual IEnumerable<IAuthorizeData> GetAuthorize()
        => [];

    public virtual Versioning GetVersioningInfo()
        => Versioning.For(version: 1);

    public virtual OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Description = string.Empty,
            GroupName = string.Empty,
            Name = GetType().Name,
            Produces = [],
            Summary = string.Empty,
            Tags = []
        };
}