using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Defines the contract for building minimal endpoints.
/// </summary>
public interface IEndpointBuilder {
    /// <summary>
    /// Configures GET for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">The route pattern.</param>
    /// <param name="handler">The endpoint handler.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Get([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    /// Configures POST for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">The route pattern.</param>
    /// <param name="handler">The endpoint handler.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Post([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    /// Configures PUT for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">The route pattern.</param>
    /// <param name="handler">The endpoint handler.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Put([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    /// Configures DELETE for the endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routePattern">The route pattern.</param>
    /// <param name="handler">The endpoint handler.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Delete([StringSyntax(Constants.ROUTE_STRING_SYNTAX)] string routePattern, Delegate handler);

    /// <summary>
    /// Sets the route prefix for the endpoint.
    /// </summary>
    /// <param name="routePrefix">The route prefix.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    /// To avoid conflicts, do not use route pattern in the prefix.
    /// </remarks>
    IEndpointBuilder WithRoutePrefix(string routePrefix);

    /// <summary>
    /// Sets the name of the endpointBase.
    /// </summary>
    /// <param name="name">The endpointBase name.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithName(string name);

    /// <summary>
    /// Sets the display name of the endpointBase.
    /// </summary>
    /// <param name="displayName">The display name.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithDisplayName(string displayName);

    /// <summary>
    /// Sets the description of the endpointBase.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithDescription(string description);

    /// <summary>
    /// Sets the summary of the endpointBase.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithSummary(string summary);

    /// <summary>
    /// Sets the tags for the endpointBase.
    /// </summary>
    /// <param name="tags">The tags.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithTags(params string[] tags);

    /// <summary>
    /// Sets the request timeout for the endpointBase using a named policy.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithRequestTimeout(string policyName);

    /// <summary>
    /// Configures the endpointBase to accept a specific request type with a primary content type and optional additional content types.
    /// </summary>
    /// <typeparam name="TRequestType">The <see cref="Type"/> of the request body.</typeparam>
    /// <param name="isOptional">Indicates whether the request content type is optional.</param>
    /// <param name="contentType">The primary content type that the endpointBase accepts, such as "application/json".</param>
    /// <param name="additionalContentTypes">Additional content types that the endpointBase can accept, specified as an array of strings.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Accepts<TRequestType>(bool isOptional, string contentType, params string[] additionalContentTypes)
        where TRequestType : notnull;

    /// <summary>
    /// Configures the endpointBase to accept a specific request type with a primary content type and optional additional content types.
    /// </summary>
    /// <param name="requestType">The <see cref="Type"/> of the request body.</param>
    /// <param name="isOptional">Indicates whether the request content type is optional.</param>
    /// <param name="contentType">The primary content type that the endpointBase accepts, such as "application/json".</param>
    /// <param name="additionalContentTypes">Additional content types that the endpointBase can accept, specified as an array of strings.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Accepts(Type requestType, bool isOptional, string contentType, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpointBase to specify the type of response it produces, along with the associated status code and
    /// content types.
    /// </summary>
    /// <remarks>Use this method to document the expected response type and status code for an endpointBase, which
    /// can help tools like Swagger generate accurate API documentation. Specifying multiple content types allows the
    /// endpointBase to support content negotiation.</remarks>
    /// <typeparam name="TResponse">The type of the response that the endpointBase produces.</typeparam>
    /// <param name="statusCode">The HTTP status code that the endpointBase will return. The default is <see cref="StatusCodes.Status200OK"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <param name="additionalContentTypes">Additional content types that the endpointBase can produce, specified as an array of strings.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Produces<TResponse>(int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpointBase to specify the type of response it produces, along with the associated status code and
    /// content types.
    /// </summary>
    /// <remarks>Use this method to document the expected response type and status code for an endpointBase, which
    /// can help tools like Swagger generate accurate API documentation. Specifying multiple content types allows the
    /// endpointBase to support content negotiation.</remarks>
    /// <param name="responseType">The type of the response that the endpointBase produces.</param>
    /// <param name="statusCode">The HTTP status code that the endpointBase will return. The default is <see cref="StatusCodes.Status200OK"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <param name="additionalContentTypes">Additional content types that the endpointBase can produce, specified as an array of strings.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder Produces(Type responseType, int statusCode = StatusCodes.Status200OK, string? contentType = null, params string[] additionalContentTypes);

    /// <summary>
    /// Configures the endpointBase to produce a problem details response for error handling, with a specified status code and content type.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that the endpointBase will return. The default is <see cref="StatusCodes.Status500InternalServerError"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError, string? contentType = null);

    /// <summary>
    /// Configures the endpointBase to produce a validation problem details response for error handling, with a specified status code and content type.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that the endpointBase will return. The default is <see cref="StatusCodes.Status400BadRequest"/>.</param>
    /// <param name="contentType">The primary content type of the response, such as "application/json". If <see langword="null"/>, the default content type is used.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest, string? contentType = null);

    /// <summary>
    /// Configures the endpointBase to use a specific filter type.
    /// </summary>
    /// <typeparam name="TEndpointFilter">Type of the endpointBase filter.</typeparam>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithFilter<TEndpointFilter>() where TEndpointFilter : IEndpointFilter;

    /// <summary>
    /// Configures the endpointBase to use anti-forgery protection.
    /// </summary>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithAntiForgery();

    /// <summary>
    /// Configures the endpointBase to use rate limiting.
    /// </summary>
    /// <param name="policyName">The name of the rate limiting policy to apply.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithRateLimiting(string policyName);

    /// <summary>
    /// Configures the endpointBase to allow anonymous access.
    /// </summary>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder AllowAnonymous();

    /// <summary>
    /// Configures the endpointBase to require authorization using the specified policy name.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithAuthorization(string policyName);

    /// <summary>
    /// Configures the endpointBase to use a specific CORS policy by name.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithCorsPolicy(string policyName);

    /// <summary>
    /// Configures the version for the endpointBase to use.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="stability">The stability of the endpoint.</param>
    /// <returns>
    /// The current <see cref="IEndpointBuilder"/> instance so other actions can be chained.
    /// </returns>
    IEndpointBuilder WithVersion(int version, Stability stability = Stability.Stable);
}