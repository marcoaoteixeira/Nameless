using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web.Api;

public interface IEndpoint {
    /// <summary>
    /// Gets the endpoint name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the endpoint summary.
    /// </summary>
    string Summary { get; }

    /// <summary>
    /// Gets the endpoint description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the endpoint group.
    /// </summary>
    string Group { get; }

    /// <summary>
    /// Gets the endpoint version.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Maps the endpoint.
    /// </summary>
    /// <param name="builder">The endpoint route builder instance.</param>
    /// <returns>
    /// The current <see cref="IEndpointConventionBuilder"/> instance so other mapping actions can be chained.
    /// </returns>
    IEndpointConventionBuilder Map(IEndpointRouteBuilder builder);
}