using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

internal static class NamedTypeSymbolExtensions {
    internal static bool IsAssignableTo(this INamedTypeSymbol self, string baseTypeFullyQualifiedName) {
        var current = self;

        while (current is not null) {
            if (current.ToDisplayString() == baseTypeFullyQualifiedName) {
                return true;
            }

            current = current.BaseType;
        }
        return false;
    }

    internal static AttributeData? GetEndpointAttribute(this INamedTypeSymbol self) {
        foreach (var attr in self.GetAttributes()) {
            var attrClass = attr.AttributeClass;
            if (attrClass is null) {
                continue;
            }

            if (attrClass.IsGenericType && attrClass.ConstructedFrom.ToDisplayString() is FQN.ENDPOINT_ATTRIBUTE) {
                return attr;
            }
        }

        return null;
    }

    internal static AttributeData? GetAcceptsAttribute(this INamedTypeSymbol self) {
        foreach (var attr in self.GetAttributes()) {
            if (attr.AttributeClass is { } attributeClass && attributeClass.IsAssignableTo(FQN.ACCEPTS_ATTRIBUTE)) {
                return attr;
            }
        }

        return null;
    }

    internal static AttributeData? GetEndpointSummaryAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.ENDPOINT_SUMMARY_ATTRIBUTE);
    }

    internal static AttributeData? GetEndpointDescriptionAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.ENDPOINT_DESCRIPTION_ATTRIBUTE);
    }

    internal static AttributeData? GetAuthorizeAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.AUTHORIZE_ATTRIBUTE);
    }

    internal static AttributeData? GetAllowsAnonymousAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.ALLOWS_ANONYMOUS_ATTRIBUTE);
    }

    internal static AttributeData? GetEnableCorsAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.ENABLE_CORS_ATTRIBUTE);
    }

    internal static AttributeData? GetEnableRateLimitingAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.ENABLE_RATE_LIMITING_ATTRIBUTE);
    }

    internal static AttributeData? GetOutputCacheAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.OUTPUT_CACHE_ATTRIBUTE);
    }

    internal static AttributeData? GetRequestTimeoutAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.REQUEST_TIMEOUT_ATTRIBUTE);
    }

    internal static AttributeData? GetDisableHttpMetricsAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.DISABLE_HTTP_METRICS_ATTRIBUTE);
    }

    internal static AttributeData? GetUseAntiforgeryAttribute(this INamedTypeSymbol self) {
        return self.GetAttribute(FQN.USE_ANTI_FORGERY_ATTRIBUTE);
    }

    private static AttributeData? GetAttribute(this INamedTypeSymbol self, string fullyQualifiedName) {
        return self.GetAttributes()
                   .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == fullyQualifiedName);
    }
}
