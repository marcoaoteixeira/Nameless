using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

internal static class NamedTypeSymbolExtensions {
    internal static ITypeSymbol? GetTypeArgument(this INamedTypeSymbol? self, int index) {
        return self is not null && index < self.TypeArguments.Length
            ? self.TypeArguments[index]
            : null;
    }
    
    internal static AttributeData? GetEndpointAttribute(this INamedTypeSymbol self) {
        foreach (var attr in self.GetAttributes()) {
            var attrClass = attr.AttributeClass;
            if (attrClass is null) { continue; }

            if (attrClass.IsGenericType && attrClass.ConstructedFrom.ToDisplayString() is NewFQN.EndpointAttribute) {
                return attr;
            }
        }

        return null;
    }

    internal static AttributeData? GetEndpointGroupingAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.EndpointGroupingAttribute);
    }

    internal static AttributeData? GetDisableAntiforgeryAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableAntiforgeryAttribute);
    }

    internal static AttributeData? GetAllowAnonymousAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.AllowAnonymousAttribute);
    }

    internal static AttributeData? GetUseAuthorizationAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.UseAuthorizationAttribute);
    }

    internal static AttributeData? GetAllowCookieRedirectAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.AllowCookieRedirectAttribute);
    }

    internal static AttributeData? GetDisableCookieRedirectAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableCookieRedirectAttribute);
    }

    internal static AttributeData? GetUseFilterAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.UseFilterAttribute);
    }

    internal static AttributeData? GetDisableHttpMetricsAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableHttpMetricsAttribute);
    }

    internal static AttributeData? GetDisableOutputCacheAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableOutputCacheAttribute);
    }

    internal static AttributeData? GetUseOutputCacheAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.UseOutputCacheAttribute);
    }

    internal static AttributeData? GetProducesAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.ProducesAttribute);
    }

    internal static AttributeData? GetProducesProblemAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.ProducesProblemAttribute);
    }

    internal static AttributeData? GetProducesValidationProblemAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.ProducesValidationProblemAttribute);
    }

    internal static AttributeData? GetDisableRateLimitingAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableRateLimitingAttribute);
    }

    internal static AttributeData? GetUseRateLimitingAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.UseRateLimitingAttribute);
    }

    internal static AttributeData? GetDisableRequestTimeoutAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableRequestTimeoutAttribute);
    }

    internal static AttributeData? GetUseRequestTimeoutAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.UseRequestTimeoutAttribute);
    }

    internal static AttributeData? GetDisableValidationAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.DisableValidationAttribute);
    }

    internal static AttributeData? GetVersionAttribute(this INamedTypeSymbol self) {
        return self.GetAttributeV2(NewFQN.VersionAttribute);
    }

    private static AttributeData? GetAttributeV2(this INamedTypeSymbol self, string fullyQualifiedName) {
        return self.GetAttributes()
                   .FirstOrDefault(
                       attr => attr.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedName
                    );
    }

    private static AttributeData? GetAttribute(this INamedTypeSymbol self, string fullyQualifiedName) {
        return self.GetAttributes()
                   .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == fullyQualifiedName);
    }
}
