using Microsoft.AspNetCore.Http;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Default implementation of <see cref="IEndpointDescriptor" />.
/// </summary>
public class EndpointDescriptor : IEndpointDescriptor {
    private readonly Dictionary<Type, AcceptMetadata> _accepts = [];
    private readonly Dictionary<int, ProduceMetadata> _produces = [];
    private readonly Dictionary<Type, Action<IEndpointFilterBuilder>> _filters = [];
    private readonly List<object> _additionalMetadata = [];

    /// <inheritdoc />
    public Type EndpointType { get; }

    /// <inheritdoc />
    public string HttpMethod { get; set; } = string.Empty;

    /// <inheritdoc />
    public string RoutePattern { get; set; } = string.Empty;

    /// <inheritdoc />
    public string ActionName { get; set; } = string.Empty;

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
    public VersionMetadata Version { get; set; } = new(1, Stability.Stable);

    /// <inheritdoc />
    public string? RequestTimeoutPolicy { get; set; }

    /// <inheritdoc />
    public string? RateLimitingPolicy { get; set; }

    /// <inheritdoc />
    public string[] AuthorizationPolicies { get; set; } = [];

    /// <inheritdoc />
    public string? CorsPolicy { get; set; }

    /// <inheritdoc />
    public string? OutputCachePolicy { get; set; }

    /// <inheritdoc />
    public bool AllowAnonymous { get; set; }

    /// <inheritdoc />
    public bool UseAntiforgery { get; set; }

    /// <inheritdoc />
    public bool DisableHttpMetrics { get; set; }

    /// <inheritdoc />
    public AcceptMetadata[] Accepts => [.. _accepts.Values];

    /// <inheritdoc />
    public ProduceMetadata[] Produces => [.. _produces.Values];

    /// <inheritdoc />
    public Action<IEndpointFilterBuilder>[] Filters => [.. _filters.Values];

    /// <inheritdoc />
    public object[] AdditionalMetadata => [.. _additionalMetadata];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointDescriptor" /> class.
    /// </summary>
    /// <param name="endpointType">
    ///     Type of the endpoint.
    /// </param>
    public EndpointDescriptor(Type endpointType) {
        Guard.Against.NotAssignableFrom<IEndpoint>(endpointType);

        EndpointType = endpointType;
    }

    /// <summary>
    ///     Adds an accepts metadata to the endpoint.
    /// </summary>
    /// <param name="metadata">
    ///     The accepts metadata.
    /// </param>
    /// <remarks>
    ///     When adding an accepts metadata, the request type is used
    ///     to differentiate between different accepts.
    /// </remarks>
    public void AddAccept(AcceptMetadata metadata) {
        Guard.Against.Null(metadata);

        _accepts[metadata.RequestType] = metadata;
    }

    /// <summary>
    ///     Adds a produce metadata to the endpoint.
    /// </summary>
    /// <param name="metadata">
    ///     The produce metadata.
    /// </param>
    /// <remarks>
    ///     When adding a produce metadata, the status code is used
    ///     to differentiate between different responses.
    /// </remarks>
    public void AddProduce(ProduceMetadata metadata) {
        Guard.Against.Null(metadata);

        _produces[metadata.StatusCode] = metadata;
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
    public void AddFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _filters[typeof(TEndpointFilter)] = builder => {
            builder.Use<TEndpointFilter>();
        };
    }

    /// <summary>
    ///     Adds a metadata to the endpoint.
    /// </summary>
    /// <param name="metadata">
    ///     The metadata object.
    /// </param>
    public void AddAdditionalMetadata(object metadata) {
        _additionalMetadata.Add(Guard.Against.Null(metadata));
    }
}
