using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Helpers;
using Nameless.Web.MinimalEndpoints.Definitions.Metadata;
using Nameless.Web.OpenApi;

namespace Nameless.Web.MinimalEndpoints.Definitions;

/// <summary>
///     A builder for creating endpoint descriptors.
/// </summary>
public class EndpointDescriptorBuilder {
    private const string ROUTE_SYNTAX = "Route";

    private readonly EndpointDescriptor _descriptor;

    /// <summary>
    ///     Private constructor so that instances can only be created
    ///     by the <see cref="Create{TEndpoint}"/> method.
    /// </summary>
    /// <param name="endpointType">
    ///     The endpoint type.
    /// </param>
    private EndpointDescriptorBuilder(Type endpointType) {
        _descriptor = new EndpointDescriptor(endpointType);
    }

    /// <summary>
    ///     Creates a new instance of the
    ///     <see cref="EndpointDescriptorBuilder"/> class.
    /// </summary>
    /// <typeparam name="TEndpoint">
    ///     Type of the endpoint to create a descriptor for.
    /// </typeparam>
    /// <returns>
    ///     A new instance of the <see cref="EndpointDescriptorBuilder"/>
    ///     class.
    /// </returns>
    public static EndpointDescriptorBuilder Create<TEndpoint>()
        where TEndpoint : IEndpoint {
        return new EndpointDescriptorBuilder(typeof(TEndpoint));
    }

    /// <summary>
    ///     Creates a new instance of the
    ///     <see cref="EndpointDescriptorBuilder"/> class.
    /// </summary>
    /// <param name="endpointType">
    ///     Type of the endpoint to create a descriptor for.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="EndpointDescriptorBuilder"/>
    ///     class.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if the <paramref name="endpointType"/> does not
    ///     implement the <see cref="IEndpoint"/> interface.
    /// </exception>
    public static EndpointDescriptorBuilder Create(Type endpointType) {
        return new EndpointDescriptorBuilder(endpointType);
    }

    /// <summary>
    ///     Defines GET HTTP method and route pattern for the endpoint.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern for the endpoint.
    /// </param>
    /// <param name="actionName">
    ///     The name of the action to invoke when the endpoint is
    ///     called.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     A second call to any method that defines a route will
    ///     replace the previous route definition.
    /// </remarks>
    public EndpointDescriptorBuilder Get([StringSyntax(ROUTE_SYNTAX)] string routePattern, string actionName) {
        return SetRouteHandler(HttpMethods.Get, routePattern, actionName);
    }

    /// <summary>
    ///     Defines POST HTTP method and route pattern for the endpoint.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern for the endpoint.
    /// </param>
    /// <param name="actionName">
    ///     The name of the action to invoke when the endpoint is
    ///     called.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     A second call to any method that defines a route will
    ///     replace the previous route definition.
    /// </remarks>
    public EndpointDescriptorBuilder Post([StringSyntax(ROUTE_SYNTAX)] string routePattern, string actionName) {
        return SetRouteHandler(HttpMethods.Post, routePattern, actionName);
    }

    /// <summary>
    ///     Defines PUT HTTP method and route pattern for the endpoint.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern for the endpoint.
    /// </param>
    /// <param name="actionName">
    ///     The name of the action to invoke when the endpoint is
    ///     called.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     A second call to any method that defines a route will
    ///     replace the previous route definition.
    /// </remarks>
    public EndpointDescriptorBuilder Put([StringSyntax(ROUTE_SYNTAX)] string routePattern, string actionName) {
        return SetRouteHandler(HttpMethods.Put, routePattern, actionName);
    }

    /// <summary>
    ///     Defines DELETE HTTP method and route pattern for the endpoint.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern for the endpoint.
    /// </param>
    /// <param name="actionName">
    ///     The name of the action to invoke when the endpoint is
    ///     called.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     A second call to any method that defines a route will
    ///     replace the previous route definition.
    /// </remarks>
    public EndpointDescriptorBuilder Delete([StringSyntax(ROUTE_SYNTAX)] string routePattern, string actionName) {
        return SetRouteHandler(HttpMethods.Delete, routePattern, actionName);
    }

    /// <summary>
    ///     Sets the endpoint name.
    /// </summary>
    /// <param name="name">
    ///     The endpoint name.
    /// </param> 
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithName(string name) {
        _descriptor.Name = Throws.When.NullOrWhiteSpace(name);

        return this;
    }

    /// <summary>
    ///     Sets the group name for the endpoint.
    /// </summary>
    /// <param name="groupName">
    ///     The group name for the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithGroupName(string groupName) {
        _descriptor.GroupName = Throws.When.NullOrWhiteSpace(groupName);

        return this;
    }

    /// <summary>
    ///     Sets the display name for the endpoint.
    /// </summary>
    /// <param name="displayName">
    ///     The display name for the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithDisplayName(string displayName) {
        _descriptor.DisplayName = Throws.When.NullOrWhiteSpace(displayName);

        return this;
    }

    /// <summary>
    ///     Sets the description for the endpoint.
    /// </summary>
    /// <param name="description">
    ///     The description for the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithDescription(string description) {
        _descriptor.Description = Throws.When.NullOrWhiteSpace(description);

        return this;
    }

    /// <summary>
    ///     Sets the summary for the endpoint.
    /// </summary>
    /// <param name="summary">
    ///     The summary for the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithSummary(string summary) {
        _descriptor.Summary = Throws.When.NullOrWhiteSpace(summary);

        return this;
    }

    /// <summary>
    ///     Sets the tags for the endpoint.
    /// </summary>
    /// <param name="tags">
    ///     The tags for the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithTags(params string[] tags) {
        _descriptor.Tags = Throws.When.Null(tags);

        return this;
    }

    /// <summary>
    ///     Sets the version number and stability of the endpoint.
    /// </summary>
    /// <param name="number">
    ///     The version number of the endpoint. Must be a non-negative
    ///     integer greater than or equal to <c>1</c>.
    /// </param>
    /// <param name="stability">
    ///     The stability of the endpoint version.
    ///     Defaults to <see cref="Stability.Stable"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder WithVersion(int number, Stability stability = Stability.Stable) {
        _descriptor.Version = new VersionMetadata {
            Number = Throws.When.LowerThan(number, compare: 0, nameof(number)), Stability = stability
        };

        return this;
    }

    /// <summary>
    ///     Sets the request timeout policy for the endpoint.
    /// </summary>
    /// <param name="policyName">
    ///     The request timeout policy name.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseRequestTimeout(string policyName) {
        _descriptor.RequestTimeoutPolicy = Throws.When.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <summary>
    ///     Sets the rate limiting policy for the endpoint.
    /// </summary>
    /// <param name="policyName">
    ///     The name of the rate limiting policy to apply to the
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseRateLimiting(string policyName) {
        _descriptor.RateLimitingPolicy = Throws.When.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <summary>
    ///     Sets the authorization policies for the endpoint.
    /// </summary>
    /// <param name="policyNames">
    ///     The names of the authorization policies to apply to the
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseAuthorization(params string[] policyNames) {
        _descriptor.AuthorizationPolicies = Throws.When.Null(policyNames);

        return this;
    }

    /// <summary>
    ///     Sets the CORS policy for the endpoint.
    /// </summary>
    /// <param name="policyName">
    ///     The name of the CORS policy to apply to the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseCors(string policyName) {
        _descriptor.CorsPolicy = Throws.When.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <summary>
    ///     Sets the output cache policy for the endpoint.
    /// </summary>
    /// <param name="policyName">
    ///     The name of the output cache policy to apply to the endpoint.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseOutputCache(string policyName) {
        _descriptor.OutputCachePolicy = Throws.When.NullOrWhiteSpace(policyName);

        return this;
    }

    /// <summary>
    ///     Sets whether the endpoint allows anonymous access.
    /// </summary>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder AllowAnonymous() {
        _descriptor.AllowAnonymous = true;

        return this;
    }

    /// <summary>
    ///     Sets whether the endpoint should use anti-forgery validation
    ///     middleware.
    /// </summary>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseAntiforgery() {
        _descriptor.UseAntiforgery = true;

        return this;
    }

    /// <summary>
    ///     Sets whether to disable HTTP metrics for the endpoint.
    /// </summary>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder DisableHttpMetrics() {
        _descriptor.DisableHttpMetrics = true;

        return this;
    }

    /// <summary>
    ///     Sets the accepted content types for the endpoint.
    /// </summary>
    /// <typeparam name="TRequestType">
    ///     Type of the request body that the endpoint accepts.
    /// </typeparam>
    /// <param name="isOptional">
    ///     Whether the request body is optional or not.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint accepts.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder Accepts<TRequestType>(bool isOptional = false, params string[] contentTypes)
        where TRequestType : notnull {
        return Accepts(typeof(TRequestType), isOptional, contentTypes);
    }

    /// <summary>
    ///     Sets the accepted content types for the endpoint.
    /// </summary>
    /// <param name="requestType">
    ///     Type of the request body that the endpoint accepts.
    /// </param>
    /// <param name="isOptional">
    ///     Whether the request body is optional or not.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint accepts.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder Accepts(Type requestType, bool isOptional = false, params string[] contentTypes) {
        Throws.When.Null(requestType);
        Throws.When.Null(contentTypes);

        var innerContentTypes = contentTypes.Length == 0
            ? ["application/json"]
            : contentTypes;

        _descriptor.AddAccept(new AcceptMetadata(
            requestType,
            isOptional,
            innerContentTypes
        ));

        return this;
    }

    /// <summary>
    ///     Sets the response types that the endpoint produces.
    /// </summary>
    /// <typeparam name="TResponse">
    ///     Type of the response body that the endpoint produces.
    /// </typeparam>
    /// <param name="statusCode">
    ///     The HTTP status code that the endpoint produces.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint produces.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder Produces<TResponse>(int statusCode = StatusCodes.Status200OK,
        params string[] contentTypes) {
        return Produces(typeof(TResponse), statusCode, contentTypes);
    }

    /// <summary>
    ///     Sets the response types that the endpoint produces.
    /// </summary>
    /// <param name="responseType">
    ///     Type of the response body that the endpoint produces.
    /// </param>
    /// <param name="statusCode">
    ///     The HTTP status code that the endpoint produces.
    ///     This value must be between <c>100</c> and <c>499</c>.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint produces.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder Produces(Type? responseType = null, int statusCode = StatusCodes.Status200OK,
        params string[] contentTypes) {
        Throws.When.Null(contentTypes);
        Throws.When.OutOfRange(
            statusCode,
            minimumValue: 100, // Status code "Continue"
            maximumValue: 499, // Max value for responses that are not server errors
            message:
            "The status code must be a value inside the range 100 (informational responses) to 499 (client error responses)."
        );

        var innerContentTypes = contentTypes.Length == 0
            ? ["application/json"]
            : contentTypes;

        _descriptor.AddProduce(new ProduceMetadata(
            responseType ?? typeof(object),
            statusCode,
            innerContentTypes
        ));

        return this;
    }

    /// <summary>
    ///     Sets the response types for problems that the endpoint produces.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code that the endpoint produces.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     The value of <paramref name="statusCode"/> must be between
    ///     the valid range values for server error responses. Which
    ///     are values from <c>500</c> to <c>599</c>.
    /// </remarks>
    public EndpointDescriptorBuilder ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError) {
        Throws.When.OutOfRange(
            statusCode,
            minimumValue: 500, // Status codes "Internal Server Error"
            maximumValue: 599, // Max value for server error responses
            message: "The status code must be a value inside the range 500 to 599 (server error responses)."
        );

        _descriptor.AddProduce(new ProduceMetadata(
            typeof(ProblemDetails),
            statusCode,
            ["application/problem+json"]
        ));

        return this;
    }

    /// <summary>
    ///     Sets the response types for validation problems that the endpoint
    ///     produces.
    /// </summary>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder ProducesValidationProblem() {
        return Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json");
    }

    /// <summary>
    ///     Sets a filter for the endpoint.
    /// </summary>
    /// <typeparam name="TEndpointFilter">
    ///     Type of the endpoint filter to use.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public EndpointDescriptorBuilder UseFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter {
        _descriptor.AddFilter<TEndpointFilter>();

        return this;
    }

    public EndpointDescriptorBuilder WithAdditionalMetadata(object value) {
        Throws.When.Null(value);

        _descriptor.AddAdditionalMetadata(value);

        return this;
    }

    /// <summary>
    ///     Builds the endpoint descriptor for the endpoint mapping.
    /// </summary>
    /// <returns>
    ///     The endpoint descriptor for the endpoint mapping.
    /// </returns>
    public IEndpointDescriptor Build() {
        EnsureRoute();
        EnsureHandler();
        EnsureDescriptor();

        return _descriptor;
    }

    private EndpointDescriptorBuilder SetRouteHandler(string httpMethod, string routePattern, string actionName) {
        _descriptor.HttpMethod = Throws.When.NullOrWhiteSpace(httpMethod);
        _descriptor.RoutePattern = Throws.When.NullOrWhiteSpace(routePattern);
        _descriptor.ActionName = Throws.When.NullOrWhiteSpace(actionName);

        return this;
    }

    private void EnsureRoute() {
        if (string.IsNullOrWhiteSpace(_descriptor.HttpMethod)) {
            throw new InvalidOperationException(message: "HTTP method must be specified.");
        }

        if (string.IsNullOrWhiteSpace(_descriptor.RoutePattern)) {
            throw new InvalidOperationException(message: "Route pattern must be specified.");
        }

        if (string.IsNullOrWhiteSpace(_descriptor.ActionName)) {
            throw new InvalidOperationException(message: "Action name must be specified.");
        }
    }

    private void EnsureHandler() {
        var handler = _descriptor.EndpointType
                                 .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                 .SingleOrDefault(method => method.Name == _descriptor.ActionName)
                      ?? throw new InvalidOperationException(
                          $"Action '{_descriptor.ActionName}' not found in endpoint '{_descriptor.EndpointType.Name}'.");

        var returnType = typeof(Task<IResult>);
        if (!returnType.IsAssignableFrom(handler.ReturnType)) {
            throw new InvalidOperationException(
                $"The return type of the endpoint action must be assignable from '{returnType.GetPrettyName()}'.");
        }

        var delegateMetadataResult = RequestDelegateFactory.InferMetadata(
            handler,
            new RequestDelegateFactoryOptions {
                RouteParameterNames = RouteHelper.GetRouteParameters(_descriptor.RoutePattern)
            }
        );

        foreach (var delegateMetadata in delegateMetadataResult.EndpointMetadata) {
            if (delegateMetadata is ProducesResponseTypeMetadata produces && produces.Type == typeof(IResult)) {
                continue;
            }

            WithAdditionalMetadata(delegateMetadata);
        }

        WithAdditionalMetadata(new EndpointHandlerMetadata(handler));
    }

    private void EnsureDescriptor() {
        WithAdditionalMetadata(new EndpointDescriptorMetadata(_descriptor));
    }
}