namespace Nameless.Web.Generators.Infrastructure;

public enum AttributeDefinitions {
    None,

    Endpoint,
    EndpointGroup,

    DisableAntiforgery,

    AllowAnonymous,
    Authorize,

    DisableCookieRedirect,
    AllowCookieRedirect,

    DisableCors,
    EnableCors,

    UseFilter,

    DisableHttpMetrics,

    DisableOutputCache,
    OutputCache,

    ProducesResponse,
    ProducesProblemResponse,
    ProducesValidationProblemResponse,

    DisableRateLimiting,
    EnableRateLimiting,

    DisableRequestTimeout,
    RequestTimeout,

    DisableValidation,
    EnableValidation,

    Deprecate
}