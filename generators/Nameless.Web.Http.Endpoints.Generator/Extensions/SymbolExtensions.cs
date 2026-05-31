using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

public static class SymbolExtensions {
    extension(ISymbol self) {
        public string GetFullName() {
            return self.ToDisplayString();
        }

        public string GetFullyQualifiedName() {
            return self.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

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

        public bool HasUseAuthorizationAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.UseAuthorization, out _);
        }

        public bool HasAllowCookieRedirectAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.AllowCookieRedirect, out _);
        }

        public bool HasDisableCookieRedirectAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableCookieRedirect, out _);
        }

        public bool HasDisableCorsAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableCors, out _);
        }

        public bool HasUseCorsAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.UseCors, out _);
        }

        public bool HasDisableOutputCacheAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableOutputCache, out _);
        }

        public bool HasUseOutputCacheAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.UseOutputCache, out _);
        }

        public bool HasDisableRateLimitingAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableRateLimiting, out _);
        }

        public bool HasUseRateLimitingAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.UseRateLimiting, out _);
        }

        public bool HasDisableRequestTimeoutAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.DisableRequestTimeout, out _);
        }

        public bool HasUseRequestTimeoutAttribute() {
            return self.TryGetAttributes(RegexCache.Attributes.UseRequestTimeout, out _);
        }
        
        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Local
        private bool TryGetAttributes(Regex regex, out AttributeData[] output) {
            output = [.. self.GetAttributes().Where(Filter)];

            return output.Length > 0;

            bool Filter(AttributeData attributeData) {
                return attributeData.AttributeClass is not null &&
                       regex.IsMatch(attributeData.AttributeClass.GetFullyQualifiedName());
            }
        }
    }
}