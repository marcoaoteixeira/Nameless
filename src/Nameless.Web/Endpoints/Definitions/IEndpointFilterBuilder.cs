using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Describes the builder for endpoint filters.
/// </summary>
public interface IEndpointFilterBuilder {
    /// <summary>
    ///     Adds an endpoint filter of the specified type.
    /// </summary>
    /// <typeparam name="TEndpointFilter">
    ///     Type of the endpoint filter.
    /// </typeparam>
    IEndpointFilterBuilder Use<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter;
}