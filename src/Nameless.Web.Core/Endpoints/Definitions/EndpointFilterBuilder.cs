using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Default implementation of the <see cref="IEndpointFilterBuilder"/>
///     interface.
/// </summary>
public class EndpointFilterBuilder : IEndpointFilterBuilder {
    private readonly RouteHandlerBuilder _routeHandlerBuilder;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointFilterBuilder"/> class.
    /// </summary>
    /// <param name="routeHandlerBuilder">
    ///     The route handler builder.
    /// </param>
    public EndpointFilterBuilder(RouteHandlerBuilder routeHandlerBuilder) {
        _routeHandlerBuilder = Guard.Against.Null(routeHandlerBuilder);
    }

    /// <inheritdoc />
    public IEndpointFilterBuilder Use<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _routeHandlerBuilder.AddEndpointFilter<TEndpointFilter>();

        return this;
    }
}