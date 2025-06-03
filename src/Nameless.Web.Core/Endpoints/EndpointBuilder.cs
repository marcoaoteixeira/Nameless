using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Default implementation of <see cref="IEndpointBuilder"/> for building endpoints in a minimal API style.
/// </summary>
public class EndpointBuilder : IEndpointBuilder {
    private readonly HashSet<AcceptMetadata> _accepts = [];
    private readonly HashSet<ProduceMetadata> _produces = [];
    private readonly HashSet<string> _rateLimitingPolicies = [];
    private readonly HashSet<string> _authorizationPolicies = [];
    private readonly HashSet<string> _corsPolicies = [];
    private readonly HashSet<Type> _filters = [];

    /// <summary>
    /// Gets the HTTP method for the endpoint.
    /// </summary>
    public string HttpMethod { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the route suffix for the endpoint, if any.
    /// </summary>
    public string? RouteSuffix { get; private set; }

    /// <summary>
    /// Gets the route template for the endpoint.
    /// </summary>
    public string Route { get; private set; } = string.Empty;

    /// <summary>
    /// The handler delegate for the endpoint.
    /// </summary>
    public Delegate? Handler { get; private set; }

    /// <summary>
    /// Gets the name of the endpoint.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

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
    /// Gets the group name for the endpoint, if any.
    /// </summary>
    public string? GroupName { get; private set; }

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
    /// Gets the collection of rate limiting policies applied to the endpoint.
    /// </summary>
    public IEnumerable<string> RateLimitingPolicies => _rateLimitingPolicies;

    /// <summary>
    /// Gets the collection of authorization policies applied to the endpoint.
    /// </summary>
    public IEnumerable<string> AuthorizationPolicies => _authorizationPolicies;

    /// <summary>
    /// Gets the collection of CORS policies applied to the endpoint.
    /// </summary>
    public IEnumerable<string> CorsPolicies => _corsPolicies;

    /// <summary>
    /// Gets the collection of filters applied to the endpoint.
    /// </summary>
    public IEnumerable<Type> Filters => _filters;

    /// <summary>
    /// Whether to use anti-forgery tokens for the endpoint.
    /// </summary>
    public bool UseAntiForgery { get; private set; }

    /// <summary>
    /// Whether to allow anonymous access to the endpoint.
    /// </summary>
    public bool UseAllowAnonymous { get; private set; }

    /// <summary>
    /// Gets the version set for the endpoint, if any.
    /// </summary>
    public string? VersionSet { get; private set; }

    /// <summary>
    /// Gets the version of the endpoint.
    /// </summary>
    public int Version { get; private set; } = 1;

    /// <summary>
    /// Whether the endpoint is deprecated.
    /// </summary>
    public bool Deprecated { get; private set; }

    /// <summary>
    /// Gets the version to which this endpoint maps, if any.
    /// </summary>
    public int MapToVersion { get; private set; }

    /// <summary>
    /// Retrieves the full route for the endpoint, including versioning and any suffix.
    /// </summary>
    /// <returns>
    /// The full route string for the endpoint, formatted as "api/v{v:apiVersion}/{RouteSuffix}{Route}".
    /// </returns>
    public string GetRoute() {
        if (string.IsNullOrWhiteSpace(Route)) { return string.Empty; }

        var baseRoute = "api/v{v:apiVersion}";

        if (!string.IsNullOrWhiteSpace(RouteSuffix)) {
            baseRoute += $"/{RouteSuffix}";
        }

        return $"{baseRoute}{Route}";
    }

    /// <inheritdoc />
    public IEndpointBuilder Get([StringSyntax("Route")] string routeTemplate, Delegate handler) {
        return SetRouteHandler(HttpMethods.Get, routeTemplate, handler);
    }

    /// <inheritdoc />
    public IEndpointBuilder Post([StringSyntax("Route")] string routeTemplate, Delegate handler) {
        return SetRouteHandler(HttpMethods.Post, routeTemplate, handler);
    }

    /// <inheritdoc />
    public IEndpointBuilder Put([StringSyntax("Route")] string routeTemplate, Delegate handler) {
        return SetRouteHandler(HttpMethods.Put, routeTemplate, handler);
    }

    /// <inheritdoc />
    public IEndpointBuilder Delete([StringSyntax("Route")] string routeTemplate, Delegate handler) {
        return SetRouteHandler(HttpMethods.Delete, routeTemplate, handler);
    }

    /// <inheritdoc />
    public IEndpointBuilder WithRouteSuffix(string routeSuffix) {
        RouteSuffix = Prevent.Argument.NullOrWhiteSpace(routeSuffix);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithName(string name) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithDisplayName(string displayName) {
        DisplayName = Prevent.Argument.NullOrWhiteSpace(displayName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithDescription(string description) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithSummary(string summary) {
        Summary = Prevent.Argument.NullOrWhiteSpace(summary);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithGroupName(string groupName) {
        GroupName = Prevent.Argument.NullOrWhiteSpace(groupName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithTags(params string[] tags) {
        Tags = Prevent.Argument.Null(tags);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithRequestTimeout(string policyName) {
        RequestTimeoutPolicy = Prevent.Argument.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder Accepts<TRequestType>(bool isOptional, string contentType, params string[] additionalContentTypes) where TRequestType : notnull {
        return Accepts(typeof(TRequestType), isOptional, contentType, additionalContentTypes);
    }

    /// <inheritdoc />
    public IEndpointBuilder Accepts(Type requestType, bool isOptional, string contentType, params string[] additionalContentTypes) {
        _accepts.Add(new AcceptMetadata(
            Prevent.Argument.Null(requestType),
            Prevent.Argument.NullOrWhiteSpace(contentType),
            isOptional,
            Prevent.Argument.Null(additionalContentTypes)
        ));

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder Produces<TResponse>(int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes) {
        return Produces(typeof(TResponse), statusCode, contentType, additionalContentTypes);
    }

    /// <inheritdoc />
    public IEndpointBuilder Produces(Type responseType, int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes) {
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
    public IEndpointBuilder ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError, string? contentType = null) {
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
    public IEndpointBuilder ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest, string? contentType = null) {
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
    public IEndpointBuilder WithFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _filters.Add(typeof(TEndpointFilter));

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithAntiForgery() {
        UseAntiForgery = true;

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithRateLimiting(string policyName) {
        _rateLimitingPolicies.Add(Prevent.Argument.NullOrWhiteSpace(policyName));

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder AllowAnonymous() {
        UseAllowAnonymous = true;

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithAuthorization(string policyName) {
        _authorizationPolicies.Add(Prevent.Argument.NullOrWhiteSpace(policyName));

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithCorsPolicy(string policyName) {
        _corsPolicies.Add(Prevent.Argument.NullOrWhiteSpace(policyName));

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithVersionSet(string versionSet) {
        VersionSet = Prevent.Argument.NullOrWhiteSpace(versionSet);

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithVersion(int version, bool deprecated = false) {
        Version = Prevent.Argument.LowerThan(version, minValue: 1);
        Deprecated = deprecated;

        return this;
    }

    /// <inheritdoc />
    public IEndpointBuilder WithVersionMap(int mapToVersion) {
        MapToVersion = Prevent.Argument.LowerThan(mapToVersion, minValue: 1);

        return this;
    }

    private IEndpointBuilder SetRouteHandler(string httpMethod, string routeTemplate, Delegate handler) {
        Route = Prevent.Argument.NullOrWhiteSpace(routeTemplate);
        HttpMethod = Prevent.Argument.NullOrWhiteSpace(httpMethod);
        Handler = Prevent.Argument.Null(handler);

        return this;
    }

    /// <summary>
    /// Represents metadata about the accepted request type for an endpoint.
    /// </summary>
    /// <param name="RequestType">Type of the request.</param>
    /// <param name="ContentType">Content type.</param>
    /// <param name="IsOptional">Whether the accept metadata is optional.</param>
    /// <param name="AdditionalContentTypes">Additional content types.</param>
    public record AcceptMetadata(
        Type RequestType,
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
    public record ProduceMetadata(
        Type? ResponseType,
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