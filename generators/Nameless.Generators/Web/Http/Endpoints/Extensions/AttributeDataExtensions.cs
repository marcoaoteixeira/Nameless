using Microsoft.CodeAnalysis;
using Nameless.Generators.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Generators.Web.Http.Endpoints;

public static class AttributeDataExtensions {
    extension(AttributeData self) {
        public AttributeDefinitions GetAttributeDefinition() {
            var fqn = self.AttributeClass?.GetFullyQualifiedName();
            if (string.IsNullOrWhiteSpace(fqn)) { return AttributeDefinitions.None; }

            if (RegexCache.Attributes.Endpoint.IsMatch(fqn)) {
                return AttributeDefinitions.Endpoint;
            }

            if (RegexCache.Attributes.EndpointGroup.IsMatch(fqn)) {
                return AttributeDefinitions.EndpointGroup;
            }

            if (RegexCache.Attributes.DisableAntiforgery.IsMatch(fqn)) {
                return AttributeDefinitions.DisableAntiforgery;
            }

            if (RegexCache.Attributes.AllowAnonymous.IsMatch(fqn)) {
                return AttributeDefinitions.AllowAnonymous;
            }

            if (RegexCache.Attributes.Authorize.IsMatch(fqn)) {
                return AttributeDefinitions.Authorize;
            }

            if (RegexCache.Attributes.DisableCookieRedirect.IsMatch(fqn)) {
                return AttributeDefinitions.DisableCookieRedirect;
            }

            if (RegexCache.Attributes.AllowCookieRedirect.IsMatch(fqn)) {
                return AttributeDefinitions.AllowCookieRedirect;
            }

            if (RegexCache.Attributes.DisableCors.IsMatch(fqn)) {
                return AttributeDefinitions.DisableCors;
            }

            if (RegexCache.Attributes.EnableCors.IsMatch(fqn)) {
                return AttributeDefinitions.EnableCors;
            }

            if (RegexCache.Attributes.UseFilter.IsMatch(fqn)) {
                return AttributeDefinitions.UseFilter;
            }

            if (RegexCache.Attributes.DisableHttpMetrics.IsMatch(fqn)) {
                return AttributeDefinitions.DisableHttpMetrics;
            }

            if (RegexCache.Attributes.DisableOutputCache.IsMatch(fqn)) {
                return AttributeDefinitions.DisableOutputCache;
            }

            if (RegexCache.Attributes.OutputCache.IsMatch(fqn)) {
                return AttributeDefinitions.OutputCache;
            }

            if (RegexCache.Attributes.ProducesResponse.IsMatch(fqn)) {
                return AttributeDefinitions.ProducesResponse;
            }

            if (RegexCache.Attributes.ProducesProblemResponse.IsMatch(fqn)) {
                return AttributeDefinitions.ProducesProblemResponse;
            }

            if (RegexCache.Attributes.ProducesValidationProblemResponse.IsMatch(fqn)) {
                return AttributeDefinitions.ProducesValidationProblemResponse;
            }

            if (RegexCache.Attributes.DisableRateLimiting.IsMatch(fqn)) {
                return AttributeDefinitions.DisableRateLimiting;
            }

            if (RegexCache.Attributes.EnableRateLimiting.IsMatch(fqn)) {
                return AttributeDefinitions.EnableRateLimiting;
            }

            if (RegexCache.Attributes.DisableRequestTimeout.IsMatch(fqn)) {
                return AttributeDefinitions.DisableRequestTimeout;
            }

            if (RegexCache.Attributes.RequestTimeout.IsMatch(fqn)) {
                return AttributeDefinitions.RequestTimeout;
            }

            if (RegexCache.Attributes.DisableValidation.IsMatch(fqn)) {
                return AttributeDefinitions.DisableValidation;
            }

            if (RegexCache.Attributes.EnableValidation.IsMatch(fqn)) {
                return AttributeDefinitions.EnableValidation;
            }

            if (RegexCache.Attributes.Deprecate.IsMatch(fqn)) {
                return AttributeDefinitions.Deprecate;
            }

            return AttributeDefinitions.None;
        }
    }
}
