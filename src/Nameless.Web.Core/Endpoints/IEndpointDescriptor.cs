using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Defines the contract for building minimal endpoints.
/// </summary>
public interface IEndpointDescriptor {
    /// <summary>
    ///     Configures GET for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern.
    /// </param>
    /// <param name="handler">
    ///     The endpoint handler.
    /// </param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Get([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    ///     Configures POST for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern.
    /// </param>
    /// <param name="handler">
    ///     The endpoint handler.
    /// </param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Post([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    ///     Configures PUT for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern.
    /// </param>
    /// <param name="handler">
    ///     The endpoint handler.
    /// </param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Put([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    /// Configures DELETE for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">The route pattern.</param>
    /// <param name="handler">The endpoint handler.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Delete([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    ///     Sets the group name for the endpoint. This will act like a route
    ///     prefix.
    /// </summary>
    /// <param name="groupName">
    ///     The route prefix.
    /// </param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithGroupName(string groupName);

    /// <summary>
    /// Sets the name of the endpoint.
    /// </summary>
    /// <param name="name">The endpoint name.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithName(string name);

    /// <summary>
    /// Sets the display name of the endpoint.
    /// </summary>
    /// <param name="displayName">The display name.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithDisplayName(string displayName);

    /// <summary>
    /// Sets the description of the endpoint.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithDescription(string description);

    /// <summary>
    /// Sets the summary of the endpoint.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithSummary(string summary);

    /// <summary>
    /// Sets the tags for the endpoint.
    /// </summary>
    /// <param name="tags">The tags.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithTags(params string[] tags);

    /// <summary>
    /// Sets the request timeout for the endpoint using a named policy.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithRequestTimeout(string policyName);

    /// <summary>
    /// Configures the endpoint to accept a specific request type with a primary content type and optional additional content types.
    /// </summary>
    /// <typeparam name="TRequestType">The <see cref="Type"/> of the request body.</typeparam>
    /// <param name="isOptional">Indicates whether the request content type is optional.</param>
    /// <param name="contentType">The primary content type that the endpoint accepts, such as "application/json".</param>
    /// <param name="additionalContentTypes">Additional content types that the endpoint can accept, specified as an array of strings.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Accepts<TRequestType>(bool isOptional, string contentType, params string[] additionalContentTypes)
        where TRequestType : notnull;

    /// <summary>
    /// Configures the endpoint to accept a specific request type with a primary content type and optional additional content types.
    /// </summary>
    /// <param name="requestType">The <see cref="Type"/> of the request body.</param>
    /// <param name="isOptional">Indicates whether the request content type is optional.</param>
    /// <param name="contentType">The primary content type that the endpoint accepts, such as "application/json".</param>
    /// <param name="additionalContentTypes">Additional content types that the endpoint can accept, specified as an array of strings.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Accepts(Type requestType, bool isOptional, string contentType, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpoint to specify the type of response it produces, along with the associated status code and
    /// content types.
    /// </summary>
    /// <remarks>Use this method to document the expected response type and status code for an endpoint, which
    /// can help tools like Swagger generate accurate API documentation. Specifying multiple content types allows the
    /// endpoint to support content negotiation.</remarks>
    /// <typeparam name="TResponse">The type of the response that the endpoint produces.</typeparam>
    /// <param name="statusCode">The HTTP status code that the endpoint will return. The default is <see cref="StatusCodes.Status200OK"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <param name="additionalContentTypes">Additional content types that the endpoint can produce, specified as an array of strings.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Produces<TResponse>(int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpoint to specify the type of response it produces, along with the associated status code and
    /// content types.
    /// </summary>
    /// <remarks>Use this method to document the expected response type and status code for an endpoint, which
    /// can help tools like Swagger generate accurate API documentation. Specifying multiple content types allows the
    /// endpoint to support content negotiation.</remarks>
    /// <param name="responseType">The type of the response that the endpoint produces.</param>
    /// <param name="statusCode">The HTTP status code that the endpoint will return. The default is <see cref="StatusCodes.Status200OK"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <param name="additionalContentTypes">Additional content types that the endpoint can produce, specified as an array of strings.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor Produces(Type responseType, int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpoint to produce a problem details response for error handling, with a specified status code and content type.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that the endpoint will return. The default is <see cref="StatusCodes.Status500InternalServerError"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <returns>
    /// The current <see cref="IEndpointDescriptor"/> instance so other actions can be chained.
    /// </returns>
    IEndpointDescriptor ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError, string? contentType = null);

    /// <summary>
    /// Configures the endpoint to produce a validation problem details response for error handling, with a specified status code and content type.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that the endpoint will return. The default is <see cref="StatusCodes.Status400BadRequest"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest, string? contentType = null);

    /// <summary>
    /// Configures the endpoint to use a specific filter type.
    /// </summary>
    /// <typeparam name="TEndpointFilter">Type of the endpoint filter.</typeparam>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithFilter<TEndpointFilter>() where TEndpointFilter : IEndpointFilter;

    /// <summary>
    /// Configures the endpoint to use anti-forgery protection.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithAntiForgery();

    /// <summary>
    /// Configures the endpoint to use rate limiting.
    /// </summary>
    /// <param name="policyName">The name of the rate limiting policy to apply.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithRateLimiting(string policyName);

    /// <summary>
    /// Configures the endpoint to allow anonymous access.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor AllowAnonymous();

    /// <summary>
    /// Configures the endpoint to require authorization using the specified policy names.
    /// </summary>
    /// <param name="policyNames">The policy names.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithAuthorization(params string[] policyNames);

    /// <summary>
    /// Configures the endpoint to use a specific CORS policy by name.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithCorsPolicy(string policyName);

    /// <summary>
    /// Configures the version for the endpoint to use.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="stability">The stability of the endpoint.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithVersion(int version, Stability stability = Stability.Stable);

    /// <summary>
    /// Marks the endpoint to be cached with the specified policy.
    /// </summary>
    /// <param name="policyName">The output cache policy name.</param>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor WithOutputCachePolicy(string policyName);

    /// <summary>
    /// Disables the HTTP metrics for the endpoint.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IEndpointDescriptor"/> instance so other
    ///     actions can be chained.
    /// </returns>
    IEndpointDescriptor DisableHttpMetrics();
}