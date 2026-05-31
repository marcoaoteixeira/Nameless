namespace Nameless.Web.Http.Endpoints.Generator.Infrastructure;

public enum AttributeDefinitions {
    None,

    Endpoint,
    EndpointGroup,

    DisableAntiforgery,

    AllowAnonymous,
    UseAuthorization,

    DisableCookieRedirect,
    AllowCookieRedirect,

    DisableCors,
    UseCors,

    UseFilter,

    DisableHttpMetrics,

    DisableOutputCache,
    UseOutputCache,

    Produces,
    ProducesProblem,
    ProducesValidationProblem,

    DisableRateLimiting,
    UseRateLimiting,

    DisableRequestTimeout,
    UseRequestTimeout,

    DisableValidation,

    Deprecate
}