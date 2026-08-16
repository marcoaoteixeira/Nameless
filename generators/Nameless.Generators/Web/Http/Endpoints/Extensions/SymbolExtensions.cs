using Microsoft.CodeAnalysis;

namespace Nameless.Generators.Web.Http.Endpoints;

public static class SymbolExtensions {
    extension(ISymbol self) {
        public bool HasEndpointAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.Endpoint, out _);
        }

        public AttributeData? GetEndpointAttribute() {
            self.TryGetAttributes(RegexCache.Attributes.Endpoint, out var inner);

            return inner.SingleOrDefault();
        }

        public bool HasEndpointGroupAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.EndpointGroup, out _);
        }

        public AttributeData? GetEndpointGroupAttribute() {
            self.TryGetAttributes(RegexCache.Attributes.EndpointGroup, out var inner);

            return inner.SingleOrDefault();
        }

        public bool HasAllowAnonymousAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.AllowAnonymous, out _);
        }

        public bool HasAuthorizeAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.Authorize, out _);
        }

        public bool HasDisableCookieRedirectAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableCookieRedirect, out _);
        }

        public bool HasAllowCookieRedirectAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.AllowCookieRedirect, out _);
        }

        public bool HasDisableCorsAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableCors, out _);
        }

        public bool HasEnableCorsAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.EnableCors, out _);
        }

        public bool HasDisableOutputCacheAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableOutputCache, out _);
        }

        public bool HasOutputCacheAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.OutputCache, out _);
        }

        public bool HasDisableRateLimitingAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableRateLimiting, out _);
        }

        public bool HasEnableRateLimitingAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.EnableRateLimiting, out _);
        }

        public bool HasDisableRequestTimeoutAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableRequestTimeout, out _);
        }

        public bool HasRequestTimeoutAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.RequestTimeout, out _);
        }

        public bool HasDisableValidationAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableValidation, out _);
        }

        public bool HasEnableValidationAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.EnableValidation, out _);
        }
    }
}