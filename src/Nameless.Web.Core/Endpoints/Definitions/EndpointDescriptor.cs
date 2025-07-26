using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Default implementation of <see cref="IEndpointDescriptor" />.
/// </summary>
public class EndpointDescriptor : IEndpointDescriptor {
    private readonly Dictionary<Type, AcceptsMetadata> _accepts = [];
    private readonly Dictionary<int, ProducesResponseTypeMetadata> _produces = [];
    private readonly Dictionary<Type, Action<IEndpointFilterBuilder>> _filters = [];

    /// <inheritdoc />
    public Type EndpointType { get; private set; } = typeof(Nothing);

    /// <inheritdoc />
    public string HttpMethod { get; set; } = string.Empty;

    /// <inheritdoc />
    public string RoutePattern { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? Name { get; set; }

    /// <inheritdoc />
    public string GroupName { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? DisplayName { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public string? Summary { get; set; }

    /// <inheritdoc />
    public string[] Tags { get; set; } = [];

    /// <inheritdoc />
    public string? RequestTimeoutPolicy { get; set; }

    /// <inheritdoc />
    public AcceptsMetadata[] Accepts => [.. _accepts.Values];

    /// <inheritdoc />
    public ProducesResponseTypeMetadata[] Produces => [.. _produces.Values];

    /// <inheritdoc />
    public Action<IEndpointFilterBuilder>[] Filters => [.. _filters.Values];

    /// <inheritdoc />
    public bool UseAntiforgery { get; set; }

    /// <inheritdoc />
    public string? RateLimitingPolicy { get; set; }

    /// <inheritdoc />
    public bool AllowAnonymous { get; set; }

    /// <inheritdoc />
    public string[] AuthorizationPolicies { get; set; } = [];

    /// <inheritdoc />
    public string? CorsPolicy { get; set; }

    /// <inheritdoc />
    public VersionMetadata Version { get; set; } = new(1, Stability.Stable);

    /// <inheritdoc />
    public string? OutputCachePolicy { get; set; }

    /// <inheritdoc />
    public bool DisableHttpMetrics { get; set; }

    /// <inheritdoc />
    public IDictionary<string, object> DataContext { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Adds an accepts metadata to the endpoint.
    /// </summary>
    /// <param name="accepts">
    ///     The accepts metadata.
    /// </param>
    /// <remarks>
    ///     When adding an accepts metadata, the request type is used
    ///     to differentiate between different accepts.
    /// </remarks>
    public void AddAccepts(AcceptsMetadata accepts) {
        Prevent.Argument.Null(accepts);

        _accepts[accepts.RequestType ?? typeof(object)] = accepts;
    }

    /// <summary>
    ///     Adds a produce metadata to the endpoint.
    /// </summary>
    /// <param name="produce">
    ///     The produce metadata.
    /// </param>
    /// <remarks>
    ///     When adding a produce metadata, the status code is used
    ///     to differentiate between different responses.
    /// </remarks>
    public void AddProduce(ProducesResponseTypeMetadata produce) {
        Prevent.Argument.Null(produce);

        _produces[produce.StatusCode] = produce;
    }

    /// <summary>
    ///     Adds an endpoint filter to the endpoint.
    /// </summary>
    /// <typeparam name="TEndpointFilter">
    ///     Type of the endpoint filter to add.
    /// </typeparam>
    /// <remarks>
    ///     When adding an endpoint filter, the filter type is used
    ///     to differentiate between different filters.
    /// </remarks>
    public void UseFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _filters[typeof(TEndpointFilter)] = builder => {
            builder.Use<TEndpointFilter>();
        };
    }

    /// <summary>
    ///     Sets the endpoint type for this descriptor.
    /// </summary>
    /// <param name="endpointType">
    ///     The type of the endpoint to set.
    /// </param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor" /> so other actions
    ///     can be chained.
    /// </returns>
    public IEndpointDescriptor SetEndpointType(Type endpointType) {
        Prevent.Argument.Null(endpointType);

        if (!typeof(IEndpoint).IsAssignableFrom(endpointType)) {
            throw new InvalidOperationException($"'{endpointType.Name}' do not implement '{nameof(IEndpoint)}'.");
        }

        EndpointType = endpointType;

        return this;
    }
}
