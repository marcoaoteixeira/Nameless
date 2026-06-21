using System.Text.RegularExpressions;

namespace Nameless.Web.Generators;

internal static class RegexCache {
    internal static class Attributes {
        internal static readonly Regex Endpoint = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.EndpointAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex EndpointGroup = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.EndpointGroupAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableAntiforgery = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Antiforgery\.DisableAntiforgeryAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex AllowAnonymous = new(
            @"^global::Microsoft\.AspNetCore\.Authorization\.AllowAnonymousAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex Authorize = new(
            @"^global::Microsoft\.AspNetCore\.Authorization\.AuthorizeAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex AllowCookieRedirect = new(
            @"^global::Microsoft\.AspNetCore\.Http\.AllowCookieRedirectAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableCookieRedirect = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.CookieRedirect\.DisableCookieRedirectAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableCors = new(
            @"^global::Microsoft\.AspNetCore\.Cors\.DisableCorsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex EnableCors = new(
            @"^global::Microsoft\.AspNetCore\.Cors\.EnableCorsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex UseFilter = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Filtering\.UseFilterAttribute(<.*>)?$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableHttpMetrics = new(
            @"^global::Microsoft\.AspNetCore\.Http\.DisableHttpMetricsAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableOutputCache = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.OutputCache\.DisableOutputCacheAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex OutputCache = new(
            @"^global::Microsoft\.AspNetCore\.OutputCaching\.OutputCacheAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex ProducesResponse = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesResponseAttribute(<.*>)?$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex ProducesProblemResponse = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesProblemResponseAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );
        
        internal static readonly Regex ProducesValidationProblemResponse = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Produces\.ProducesValidationProblemResponseAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableRateLimiting = new(
            @"^global::Microsoft\.AspNetCore\.RateLimiting\.DisableRateLimitingAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex EnableRateLimiting = new(
            @"^global::Microsoft\.AspNetCore\.RateLimiting\.EnableRateLimitingAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableRequestTimeout = new(
            @"^global::Microsoft\.AspNetCore\.Http\.Timeouts\.DisableRequestTimeoutAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex RequestTimeout = new(
            @"^global::Microsoft\.AspNetCore\.Http\.Timeouts\.RequestTimeoutAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex DisableValidation = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Validation\.DisableValidationAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex EnableValidation = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Validation\.EnableValidationAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        internal static readonly Regex Deprecate = new(
            @"^global::Nameless\.Web\.Http\.Endpoints\.Attributes\.Versioning\.DeprecateAttribute$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );
    }
}
