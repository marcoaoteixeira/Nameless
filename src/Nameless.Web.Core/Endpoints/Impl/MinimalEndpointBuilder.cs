using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web.Endpoints.Impl;

public sealed class MinimalEndpointBuilder : IMinimalEndpointBuilder {
    private readonly RouteHandlerBuilder _routeHandlerBuilder;
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;

    public MinimalEndpointBuilder(RouteHandlerBuilder routeHandlerBuilder, IEndpointRouteBuilder endpointRouteBuilder) {
        _routeHandlerBuilder = Prevent.Argument.Null(routeHandlerBuilder);
        _endpointRouteBuilder = Prevent.Argument.Null(endpointRouteBuilder);
    }

    public IMinimalEndpointBuilder WithOpenApi()
        => Configure(builder => builder.WithOpenApi());

    public IMinimalEndpointBuilder WithName(string name)
        => Configure(builder => builder.WithName(Prevent.Argument.NullOrWhiteSpace(name)));

    public IMinimalEndpointBuilder WithDisplayName(string displayName)
        => Configure(builder => builder.WithDisplayName(Prevent.Argument.NullOrWhiteSpace(displayName)));

    public IMinimalEndpointBuilder WithDescription(string description)
        => Configure(builder => builder.WithDescription(Prevent.Argument.NullOrWhiteSpace(description)));

    public IMinimalEndpointBuilder WithSummary(string summary)
        => Configure(builder => builder.WithSummary(Prevent.Argument.NullOrWhiteSpace(summary)));

    public IMinimalEndpointBuilder WithGroupName(string groupName)
        => Configure(builder => builder.WithGroupName(Prevent.Argument.NullOrWhiteSpace(groupName)));

    public IMinimalEndpointBuilder WithTags(params string[] tags)
        => Configure(builder => builder.WithTags(tags));

    public IMinimalEndpointBuilder WithRequestTimeout(TimeSpan timeout)
        => Configure(builder => builder.WithRequestTimeout(timeout));

    public IMinimalEndpointBuilder WithRequestTimeout(string policyName)
        => Configure(builder => builder.WithRequestTimeout(policyName));

    public IMinimalEndpointBuilder Accepts<TRequestType>(string contentType,
                                                        params string[] additionalContentTypes)
        where TRequestType : notnull
        => Configure(builder => builder.Accepts<TRequestType>(contentType, additionalContentTypes));

    public IMinimalEndpointBuilder Accepts(Type requestType,
                                          bool isOptional,
                                          string contentType,
                                          params string[] additionalContentTypes)
        => Configure(builder => builder.Accepts(requestType, isOptional, contentType, additionalContentTypes));

    public IMinimalEndpointBuilder Produces<TResponse>(int statusCode = StatusCodes.Status200OK,
                                                      string? contentType = null,
                                                      params string[] additionalContentTypes)
        => Configure(builder => builder.Produces<TResponse>(statusCode, contentType, additionalContentTypes));

    public IMinimalEndpointBuilder Produces(int statusCode,
                                           Type? responseType = null,
                                           string? contentType = null,
                                           params string[] additionalContentTypes)
        => Configure(builder => builder.Produces(statusCode, responseType, contentType, additionalContentTypes));

    public IMinimalEndpointBuilder ProducesProblem(int statusCode = StatusCodes.Status500InternalServerError,
                                                  string? contentType = null)
        => Configure(builder => builder.ProducesProblem(statusCode, contentType));

    public IMinimalEndpointBuilder ProducesValidationProblem(int statusCode = StatusCodes.Status400BadRequest,
                                                            string? contentType = null)
        => Configure(builder => builder.ProducesValidationProblem(statusCode, contentType));

    public IMinimalEndpointBuilder AddEndpointFilter<TEndpointFilter>()
        where TEndpointFilter : IEndpointFilter
        => Configure(builder => builder.AddEndpointFilter<TEndpointFilter>());

    public IMinimalEndpointBuilder DisableRequestTimeout()
        => Configure(builder => builder.DisableRequestTimeout());

    public IMinimalEndpointBuilder DisableAntiforgery()
        => Configure(builder => builder.DisableAntiforgery());

    public IMinimalEndpointBuilder DisableRateLimiting()
        => Configure(builder => builder.DisableRateLimiting());

    public IMinimalEndpointBuilder AllowAnonymous()
        => Configure(builder => builder.AllowAnonymous());

    public IMinimalEndpointBuilder RequireAuthorization(params string[] policyNames)
        => Configure(builder => builder.RequireAuthorization(policyNames));

    public IMinimalEndpointBuilder RequireAuthorization(Action<AuthorizationPolicyBuilder> configure)
        => Configure(builder => builder.RequireAuthorization(configure));

    public IMinimalEndpointBuilder WithApiVersionSet(string? name = null)
        => Configure((routeHandlerBuilder, endpointRouteBuilder) => {
            var apiVersionSet = endpointRouteBuilder.NewApiVersionSet(name)
                                                    .Build();

            routeHandlerBuilder.WithApiVersionSet(apiVersionSet);
        });

    public IMinimalEndpointBuilder HasApiVersion(int version)
        => Configure(builder => builder.HasApiVersion(version));

    public IMinimalEndpointBuilder HasApiVersion(ApiVersion version)
        => Configure(builder => builder.HasApiVersion(version));

    public IMinimalEndpointBuilder HasDeprecatedApiVersion(int version)
        => Configure(builder => builder.HasDeprecatedApiVersion(version));

    public IMinimalEndpointBuilder HasDeprecatedApiVersion(ApiVersion version)
        => Configure(builder => builder.HasDeprecatedApiVersion(version));

    public IMinimalEndpointBuilder MapToApiVersion(int version)
        => Configure(builder => builder.MapToApiVersion(version));

    public IMinimalEndpointBuilder MapToApiVersion(ApiVersion version)
        => Configure(builder => builder.MapToApiVersion(version));

    public IMinimalEndpointBuilder RequireCors(string policyName)
        => Configure(builder => builder.RequireCors(policyName));

    public IMinimalEndpointBuilder RequireCors(Action<CorsPolicyBuilder> configure)
        => Configure(builder => builder.RequireCors(configure));

    private MinimalEndpointBuilder Configure(Action<RouteHandlerBuilder> configure) {
        configure(_routeHandlerBuilder);

        return this;
    }

    private MinimalEndpointBuilder Configure(Action<RouteHandlerBuilder, IEndpointRouteBuilder> configure) {
        configure(_routeHandlerBuilder, _endpointRouteBuilder);

        return this;
    }
}