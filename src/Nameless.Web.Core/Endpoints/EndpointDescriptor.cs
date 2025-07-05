using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Default implementation of <see cref="IEndpointDescriptor"/> for building endpoints in a minimal API style.
/// </summary>
public sealed class EndpointDescriptor : IEndpointDescriptor {
    private const string ROUTE_VERSION_PARAM = "{v:apiVersion}";
    private const string ROUTE_VERSION_PREFIX = $"api/v{ROUTE_VERSION_PARAM}";

    private readonly HashSet<AcceptMetadata> _accepts = [];
    private readonly HashSet<ProduceMetadata> _produces = [];
    private readonly HashSet<string> _authorizationPolicies = [];
    private readonly HashSet<Type> _filters = [];

    private string? _name;

    /// <summary>
    /// Gets the endpoint type.
    /// </summary>
    public Type EndpointType { get; private set; }

    /// <summary>
    /// Gets the endpoint group name.
    /// </summary>
    public string GroupName => string.Concat(new[] { ROUTE_VERSION_PREFIX, RoutePrefix }.OfType<string>());

    /// <summary>
    /// Gets the HTTP method for the endpoint.
    /// </summary>
    public string HttpMethod { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the route pattern for the endpoint.
    /// </summary>
    public string RoutePattern { get; private set; } = string.Empty;

    /// <summary>
    /// The handler delegate for the endpoint.
    /// </summary>
    public WeakReference<Delegate>? Handler { get; private set; }

    /// <summary>
    /// Gets the route prefix for the endpoint, if any.
    /// </summary>
    public string? RoutePrefix { get; private set; }

    /// <summary>
    /// Gets the name of the endpoint.
    /// </summary>
    public string Name {
        get => string.IsNullOrWhiteSpace(_name)
            ? string.Concat(GroupName.Replace(ROUTE_VERSION_PARAM, Version.ToString()), RoutePattern)
            : _name;
        set => _name = value;
    }

    /// <summary>
    /// Gets the display name of the endpoint, if any.
    /// </summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// Gets the description of the endpoint, if any.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the summary of the endpoint, if any.
    /// </summary>
    public string? Summary { get; private set; }

    /// <summary>
    /// Gets the tags associated with the endpoint, if any.
    /// </summary>
    public string[] Tags { get; private set; } = [];

    /// <summary>
    /// GEts the request timeout policy name, if any.
    /// </summary>
    public string? RequestTimeoutPolicy { get; private set; }

    /// <summary>
    /// Gets the collection of accepted request metadata for the endpoint. 
    /// </summary>
    public IEnumerable<AcceptMetadata> AcceptMetadataCollection => _accepts;

    /// <summary>
    /// Gets the collection of produced response metadata for the endpoint.
    /// </summary>
    public IEnumerable<ProduceMetadata> ProducesMetadataCollection => _produces;

    /// <summary>
    /// Gets the collection of filters applied to the endpoint.
    /// </summary>
    public IEnumerable<Type> Filters => _filters;

    /// <summary>
    /// Whether to use anti-forgery tokens for the endpoint.
    /// </summary>
    public bool UseAntiForgery { get; private set; }

    /// <summary>
    /// Gets the rate limiting policy applied to the endpoint.
    /// </summary>
    public string? RateLimitingPolicy { get; private set; }

    /// <summary>
    /// Whether to allow anonymous access to the endpoint.
    /// </summary>
    public bool UseAllowAnonymous { get; private set; }

    /// <summary>
    /// Gets the collection of authorization policies applied to the endpoint.
    /// </summary>
    public IEnumerable<string> AuthorizationPolicies => _authorizationPolicies;

    /// <summary>
    /// Gets the CORS policy to apply to the endpoint.
    /// </summary>
    public string? CorsPolicy { get; private set; }

    /// <summary>
    /// Gets the version of the endpoint.
    /// </summary>
    public int Version { get; private set; } = 1;

    /// <summary>
    /// Gets the stability of the endpoint.
    /// </summary>
    public Stability Stability { get; private set; }

    /// <summary>
    /// Gets the output cache policy to apply to the endpoint.
    /// </summary>
    public string? OutputCachePolicy { get; private set; }

    /// <summary>
    /// Whether it should disable the HTTP metrics.
    /// </summary>
    public bool UseHttpMetrics { get; private set; } = true;

    /// <summary>
    /// Initializes a new instance of <see cref="EndpointDescriptor"/>.
    /// </summary>
    /// <param name="endpointType">The endpoint type that is associated with this descriptor.</param>
    public EndpointDescriptor(Type endpointType) {
        EndpointType = Prevent.Argument.Null(endpointType);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Get([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler) {
        return SetRouteHandler(HttpMethods.Get, routePattern, handler);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Post([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler) {
        return SetRouteHandler(HttpMethods.Post, routePattern, handler);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Put([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler) {
        return SetRouteHandler(HttpMethods.Put, routePattern, handler);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Delete([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler) {
        return SetRouteHandler(HttpMethods.Delete, routePattern, handler);
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithRoutePrefix(string routePrefix) {
        RoutePrefix = Prevent.Argument.NullOrWhiteSpace(routePrefix);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithName(string name) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithDisplayName(string displayName) {
        DisplayName = Prevent.Argument.NullOrWhiteSpace(displayName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithDescription(string description) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithSummary(string summary) {
        Summary = Prevent.Argument.NullOrWhiteSpace(summary);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithTags(params string[] tags) {
        Tags = Prevent.Argument.Null(tags);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithRequestTimeout(string policyName) {
        RequestTimeoutPolicy = Prevent.Argument.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor Accepts<TRequestType>(bool isOptional, string contentType, params string[] additionalContentTypes) where TRequestType : notnull {
        return Accepts(typeof(TRequestType), isOptional, contentType, additionalContentTypes);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Accepts(Type requestType, bool isOptional, string contentType, params string[] additionalContentTypes) {
        _accepts.Add(new AcceptMetadata(
            Prevent.Argument.Null(requestType),
            Prevent.Argument.NullOrWhiteSpace(contentType),
            isOptional,
            Prevent.Argument.Null(additionalContentTypes)
        ));

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor Produces<TResponse>(int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes) {
        return Produces(typeof(TResponse), statusCode, contentType, additionalContentTypes);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Produces(Type responseType, int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes) {
        _produces.Add(new ProduceMetadata(
            Prevent.Argument.Null(responseType),
            statusCode,
            contentType,
            Prevent.Argument.Null(additionalContentTypes),
            ProduceType.Result
        ));

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError, string? contentType = null) {
        _produces.Add(new ProduceMetadata(
            ResponseType: null,
            statusCode,
            contentType,
            AdditionalContentTypes: [],
            ProduceType.Problem
        ));

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest, string? contentType = null) {
        _produces.Add(new ProduceMetadata(
            ResponseType: null,
            statusCode,
            contentType,
            AdditionalContentTypes: [],
            ProduceType.ValidationProblem
        ));

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _filters.Add(typeof(TEndpointFilter));

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithAntiForgery() {
        UseAntiForgery = true;

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithRateLimiting(string policyName) {
        RateLimitingPolicy = Prevent.Argument.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor AllowAnonymous() {
        UseAllowAnonymous = true;

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithAuthorization(params string[] policyNames) {
        _authorizationPolicies.Clear();

        foreach (var policyName in policyNames) {
            _authorizationPolicies.Add(Prevent.Argument.NullOrWhiteSpace(policyName));
        }

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithCorsPolicy(string policyName) {
        CorsPolicy = Prevent.Argument.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithVersion(int version, Stability stability = Stability.Stable) {
        Version = Prevent.Argument.LowerThan(version, minValue: 1);
        Stability = stability;

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor WithOutputCachePolicy(string policyName) {
        OutputCachePolicy = Prevent.Argument.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointDescriptor DisableHttpMetrics() {
        UseHttpMetrics = false;

        return this;
    }

    private EndpointDescriptor SetRouteHandler(string httpMethod, string routePattern, Delegate handler) {
        RoutePattern = Prevent.Argument.NullOrWhiteSpace(routePattern);
        HttpMethod = Prevent.Argument.NullOrWhiteSpace(httpMethod);
        Handler = new WeakReference<Delegate>(Prevent.Argument.Null(handler));

        return this;
    }

    /// <summary>
    /// Represents metadata about the accepted request type for an endpoint.
    /// </summary>
    /// <param name="RequestType">Type of the request.</param>
    /// <param name="ContentType">Content type.</param>
    /// <param name="IsOptional">Whether the accept metadata is optional.</param>
    /// <param name="AdditionalContentTypes">Additional content types.</param>
    public record AcceptMetadata(Type RequestType,
                                 string ContentType,
                                 bool IsOptional,
                                 string[] AdditionalContentTypes);

    /// <summary>
    /// Represents metadata about the produced response type for an endpoint.
    /// </summary>
    /// <param name="ResponseType">The response type.</param>
    /// <param name="StatusCode">The status code.</param>
    /// <param name="ContentType">The content type.</param>
    /// <param name="AdditionalContentTypes">Additional content types.</param>
    /// <param name="Type">Whether the response is a result, a problem, or a validation problem.</param>
    public record ProduceMetadata(Type? ResponseType,
                                  int StatusCode,
                                  string? ContentType,
                                  string[] AdditionalContentTypes,
                                  ProduceType Type);

    /// <summary>
    /// Represents the type of response produced by an endpoint.
    /// </summary>
    public enum ProduceType {
        Result,

        Problem,

        ValidationProblem
    }
}