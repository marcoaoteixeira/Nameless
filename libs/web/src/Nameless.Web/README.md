# Nameless.Web

ASP.NET Core building blocks for Nameless web applications: host
bootstrapping, request correlation, authentication/authorization
helpers, CORS, output caching, rate limiting, request timeouts, OpenAPI
document transformers, health checks, and global exception handling.

## Host features

`WebHostFactory` builds a web `IHost` from `IWebHostFeature` instances
registered through `AppHostFeatureCollectionExtensions`, following the
same feature-composition pattern as `Nameless.Common`'s `HostFactory`.

## Request correlation

`HttpRequestCorrelationMiddleware` assigns/propagates a correlation ID
per request; `HttpRequestCorrelationDelegatingHandler` forwards it on
outgoing `HttpClient` calls, and
`HttpRequestCorrelationLogEventEnricher` attaches it to Serilog log
events. Read it back via `HttpContextExtensions`.

## Authentication

`IAuthenticationBuilder` / `AuthenticationBuilderWrapper` configure JWT
bearer authentication (`JsonWebTokenOptions`).
`AuthorizationForwardingHandler` forwards the current request's
authorization header on outgoing `HttpClient` calls.

## Cross-cutting middleware

- `CorsPolicyOptions` / `CorsPolicyOptionsExtensions` — named CORS policy
  registration.
- `OutputCachePolicyOptions` / `OutputCachePolicyOptionsExtensions` —
  named output-cache policy registration.
- `RateLimiterFeatureOptions` / `RateLimiterPolicyAttribute` — rate
  limiting, including `DefaultSlidingWindowRateLimiterPolicy`.
- `RequestTimeoutPolicyOptions` / `RequestTimeoutServiceCollectionExtensions`
  — per-endpoint request timeouts.
- `GlobalExceptionHandler` — converts unhandled exceptions into a
  consistent error response.

## OpenAPI

`BearerSecuritySchemeDocumentTransformer` and
`HttpQueryOperationDocumentTransformer` enrich the generated OpenAPI
document; `ScalarRegistration` / `ScalarWebApplicationExtensions` expose
a Scalar UI for it.

## Health checks and service discovery

`HealthCheckOptions` / `HealthCheckEntry` register health check entries;
`ServiceDiscoveryRegistration` wires up .NET Aspire-style service
discovery for `HttpClient`.

## Data protection

`AppDataProtectionOptions` configures ASP.NET Core Data Protection key
storage/persistence for the application.
