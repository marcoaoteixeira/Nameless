using System.Text.RegularExpressions;

namespace Nameless.Web.Http.Endpoints.Generator;

public static class RegexCache {
    public static class Attributes {
        public static readonly Regex Endpoint = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.EndpointAttribute(?:<.*>)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex EndpointGroup = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.EndpointGroupAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableAntiforgery = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Antiforgery\.DisableAntiforgeryAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex AllowAnonymous = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Authorization\.AllowAnonymousAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseAuthorization = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Authorization\.UseAuthorizationAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex AllowCookieRedirect = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.CookieRedirect\.AllowCookieRedirectAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableCookieRedirect = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.CookieRedirect\.DisableCookieRedirectAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableCors = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Cors\.DisableCorsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseCors = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Cors\.UseCorsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseFilter = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Filtering\.UseFilterAttribute(<.*>)?$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableHttpMetrics = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.HttpMetrics\.DisableHttpMetricsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableOutputCache = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.OutputCache\.DisableOutputCacheAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseOutputCache = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.OutputCache\.UseOutputCacheAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex Produces = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesAttribute(<.*>)?$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex ProducesProblem = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesProblemAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );
        
        public static readonly Regex ProducesValidationProblem = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesValidationProblemAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableRateLimiting = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.RateLimiting\.DisableRateLimitingAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseRateLimiting = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.RateLimiting\.UseRateLimitingAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableRequestTimeout = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.RequestTimeout\.DisableRequestTimeoutAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex UseRequestTimeout = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.RequestTimeout\.UseRequestTimeoutAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex DisableValidation = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Validation\.DisableValidationAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public static readonly Regex Deprecate = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Versioning\.DeprecateAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );
    }
}
