using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

public interface IMinimalEndpointBuilder {
    IMinimalEndpointBuilder WithOpenApi();
    IMinimalEndpointBuilder WithName(string name);
    IMinimalEndpointBuilder WithDisplayName(string displayName);
    IMinimalEndpointBuilder WithDescription(string description);
    IMinimalEndpointBuilder WithSummary(string summary);
    IMinimalEndpointBuilder WithGroupName(string groupName);
    IMinimalEndpointBuilder WithTags(params string[] tags);
    IMinimalEndpointBuilder WithRequestTimeout(TimeSpan timeout);
    IMinimalEndpointBuilder WithRequestTimeout(string policyName);
    IMinimalEndpointBuilder Accepts<TRequestType>(string contentType,
                                                 params string[] additionalContentTypes)
        where TRequestType : notnull;
    IMinimalEndpointBuilder Accepts(Type requestType,
                                   bool isOptional,
                                   string contentType,
                                   params string[] additionalContentTypes);
    IMinimalEndpointBuilder Produces<TResponse>(int statusCode = StatusCodes.Status200OK,
                                               string? contentType = null,
                                               params string[] additionalContentTypes);
    IMinimalEndpointBuilder Produces(int statusCode,
                                    Type? responseType = null,
                                    string? contentType = null,
                                    params string[] additionalContentTypes);
    IMinimalEndpointBuilder ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError,
                                           string? contentType = null);
    IMinimalEndpointBuilder ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest,
                                                     string? contentType = null);
    IMinimalEndpointBuilder AddEndpointFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter;
    IMinimalEndpointBuilder DisableRequestTimeout();
    IMinimalEndpointBuilder DisableAntiforgery();
    IMinimalEndpointBuilder DisableRateLimiting();
    IMinimalEndpointBuilder AllowAnonymous();
    IMinimalEndpointBuilder RequireAuthorization(params string[] policyNames);
    IMinimalEndpointBuilder RequireAuthorization(Action<AuthorizationPolicyBuilder> configure);
    IMinimalEndpointBuilder WithApiVersionSet(string? name = null);
    IMinimalEndpointBuilder HasApiVersion(int version);
    IMinimalEndpointBuilder HasApiVersion(ApiVersion version);
    IMinimalEndpointBuilder HasDeprecatedApiVersion(int version);
    IMinimalEndpointBuilder HasDeprecatedApiVersion(ApiVersion version);
    IMinimalEndpointBuilder MapToApiVersion(int version);
    IMinimalEndpointBuilder MapToApiVersion(ApiVersion version);
    IMinimalEndpointBuilder RequireCors(string policyName);
    IMinimalEndpointBuilder RequireCors(Action<CorsPolicyBuilder> configure);
}