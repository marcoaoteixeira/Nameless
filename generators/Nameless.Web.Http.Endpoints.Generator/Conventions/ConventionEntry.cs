using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;

namespace Nameless.Web.Http.Endpoints.Generator.Conventions;

/// <summary>
///     One fluent call to chain on the route handler builder.
///     Stored as a literal fragment that follows the "builder",
///     e.g.: <c>RequireCors("cors-policy")</c>.
/// </summary>
/// <param name="Call">
///     The fluent call.
/// </param>
internal readonly record struct ConventionEntry(
    string Call
);

internal static class ConventionExtractor {
    internal static ImmutableArray<ConventionEntry> Extract(ImmutableArray<AttributeData> attributes) {
        if (attributes.IsDefaultOrEmpty) { return []; }

        var result = ImmutableArray.CreateBuilder<ConventionEntry>();

        foreach (var attribute in attributes) {
            if (TryMapConvention(attribute, out var entry)) {
                result.Add(entry);
            }
        }

        return [.. result];
    }

    private static bool TryMapConvention(AttributeData attribute, out ConventionEntry output) {
        output = default;

        // if (!System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Launch();}

        // System.Diagnostics.Debugger.Break();

        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return false; }

        var name = GetFullyQualifiedName(attributeClass);

        output = name switch {
            NewFQN.DisableAntiforgeryAttribute => ExtractDisableAntiforgeryConvention(),

            NewFQN.AllowAnonymousAttribute => ExtractAllowAnonymousConvention(),
            NewFQN.UseAuthorizationAttribute => ExtractUseAuthorizationConvention(attribute),

            NewFQN.AllowCookieRedirectAttribute => ExtractAllowCookieRedirectConvention(),
            NewFQN.DisableCookieRedirectAttribute => ExtractDisableCookieRedirectConvention(),

            NewFQN.UseFilterAttribute => ExtractFilterConvention(attribute),

            NewFQN.DisableHttpMetricsAttribute => ExtractDisableHttpMetricsConvention(),

            NewFQN.DisableOutputCacheAttribute => ExtractDisableOutputCacheConvention(),
            NewFQN.UseOutputCacheAttribute => ExtractUseOutputCacheConvention(attribute),

            NewFQN.ProducesAttribute => ExtractProducesConvention(attribute),
            NewFQN.ProducesProblemAttribute => ExtractProducesProblemConvention(attribute),
            NewFQN.ProducesValidationProblemAttribute => ExtractProducesValidationProblemConvention(attribute),

            NewFQN.DisableRateLimitingAttribute => ExtractDisableRateLimitingConvention(),
            NewFQN.UseRateLimitingAttribute => ExtractUseRateLimitingConvention(attribute),

            NewFQN.DisableRequestTimeoutAttribute => ExtractDisableRequestTimeoutConvention(),
            NewFQN.UseRequestTimeoutAttribute => ExtractUseRequestTimeoutConvention(attribute),

            NewFQN.DisableValidationAttribute => ExtractDisableValidationConvention(),

            NewFQN.VersionAttribute => ExtractVersionConvention(attribute),

            NewFQN.EndpointAttribute or NewFQN.EndpointGroupingAttribute => ExtractDescriptionSummaryConvention(attribute),

            _ => default
        };

        return !string.IsNullOrWhiteSpace(output.Call);
    }

    private static ConventionEntry ExtractDisableAntiforgeryConvention() {
        return new ConventionEntry(
            Call: "DisableAntiforgery()"
        );
    }

    private static ConventionEntry ExtractAllowAnonymousConvention() {
        return new ConventionEntry(
            Call: "AllowAnonymous()"
        );
    }

    private static ConventionEntry ExtractUseAuthorizationConvention(AttributeData attribute) {
        var policyName = attribute.GetNamedArgument("PolicyName").GetValue<string?>();
        var roles = attribute.GetNamedArgument("Roles").GetValue<string?>();
        var authenticationSchemes = attribute.GetNamedArgument("AuthenticationSchemes").GetValue<string?>();

        if (policyName is null && roles is null && authenticationSchemes is null) {
            return new ConventionEntry(
                Call: "RequireAuthorization()"
            );
        }

        var properties = new List<string>();

        if (policyName is not null) {
            properties.Add($"Policy = \"{EscapeStringLiteral(policyName)}\"");
        }

        if (roles is not null) {
            properties.Add($"Roles = \"{EscapeStringLiteral(roles)}\"");
        }

        if (authenticationSchemes is not null) {
            properties.Add($"AuthenticationSchemes = \"{EscapeStringLiteral(authenticationSchemes)}\"");
        }

        return new ConventionEntry(
            Call: $"RequireAuthorization(new {NewFQN.AuthorizeAttribute}{{ {string.Join(", ", properties)} }})"
        );
    }

    private static ConventionEntry ExtractAllowCookieRedirectConvention() {
        return new ConventionEntry(
            Call: "AllowCookieRedirect()"
        );
    }

    private static ConventionEntry ExtractDisableCookieRedirectConvention() {
        return new ConventionEntry(
            Call: "DisableCookieRedirect()"
        );
    }

    private static ConventionEntry ExtractFilterConvention(AttributeData attribute) {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return default; }

        string? filter = null;

        // Generic usage [UseFilter<T>]: T is a type argument, not a ctor arg
        if (attributeClass.TypeArguments.Length > 0) {
            filter = attributeClass.TypeArguments[0].ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            );
        }

        if (attribute.TryGetConstructorArguments(index: 0, out var constant) && constant is { Kind: TypedConstantKind.Type, Value: INamedTypeSymbol filterType }) {
            filter = filterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        return filter is not null
            ? new ConventionEntry(
                Call: $"AddEndpointFilter<{filter}>()"
            )
            : default;
    }

    private static ConventionEntry ExtractDisableHttpMetricsConvention() {
        return new ConventionEntry(
            Call: "DisableHttpMetrics()"
        );
    }

    private static ConventionEntry ExtractDisableOutputCacheConvention() {
        return new ConventionEntry(
            Call: "CacheOutput(policy => policy.NoCache())"
        );
    }

    private static ConventionEntry ExtractUseOutputCacheConvention(AttributeData attribute) {
        return attribute.TryGetConstructorArguments(index: 0, out var output) && output is { Value: string }
            ? new ConventionEntry(
                Call: $"CacheOutput(\"{EscapeStringLiteral(output.Value.ToString())}\")"
            )
            : default;
    }

    private static ConventionEntry ExtractProducesConvention(AttributeData attribute) {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return default; }

        var responseType = NewFQN.Object;
        var statusCode = 200;
        var contentType = JsonContentType;

        // Generic usage [Produces<T>]: T is a type argument, not a ctor arg
        if (attributeClass.TypeArguments.Length > 0) {
            responseType = attributeClass.TypeArguments[0].ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            );

            statusCode = GetStatusCode(attribute, index: 0);
            contentType = GetContentType(attribute, index: 1);
        }
        else {
            if (attribute.TryGetConstructorArguments(index: 0, out var responseTypeConstant) &&
                responseTypeConstant is { Value: not null }) {
                responseType = ((INamedTypeSymbol)responseTypeConstant.Value).ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat
                );
            }

            statusCode = GetStatusCode(attribute, index: 1);
            contentType = GetContentType(attribute, index: 2);
        }

        return new ConventionEntry(
            Call: $"Produces<{responseType}>(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static ConventionEntry ExtractProducesProblemConvention(AttributeData attribute) {
        var statusCode = GetStatusCode(attribute, index: 0, fallback: 500);
        var contentType = GetContentType(attribute, index: 1, fallback: JsonProblemContentType);

        return new ConventionEntry(
            Call: $"ProducesProblem(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static ConventionEntry ExtractProducesValidationProblemConvention(AttributeData attribute) {
        var statusCode = GetStatusCode(attribute, index: 0, fallback: 400);
        var contentType = GetContentType(attribute, index: 1, fallback: JsonProblemContentType);

        return new ConventionEntry(
            Call: $"ProducesValidationProblem(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static int GetStatusCode(AttributeData attribute, int index, int fallback = 200) {
        return attribute.TryGetConstructorArguments(index, out var output) && output is { Value: not null }
            ? (int)output.Value
            : fallback;
    }

    private static string GetContentType(AttributeData attribute, int index, string fallback = JsonContentType) {
        return attribute.TryGetConstructorArguments(index, out var output) && output is { Value: not null }
            ? (string)output.Value
            : fallback;
    }

    private static ConventionEntry ExtractDisableRateLimitingConvention() {
        return new ConventionEntry(
            Call: "DisableRateLimiting()"
        );
    }

    private static ConventionEntry ExtractUseRateLimitingConvention(AttributeData attribute) {
        var policyName = attribute.TryGetConstructorArguments(index: 0, out var output) && output is { Value: not null }
            ? (string)output.Value
            : null;

        return policyName is not null
            ? new ConventionEntry(
                Call: $"RequireRateLimiting(\"{EscapeStringLiteral(policyName)}\")"
            )
            : default;
    }

    private static ConventionEntry ExtractDisableRequestTimeoutConvention() {
        return new ConventionEntry(
            Call: "DisableRequestTimeout()"
        );
    }

    private static ConventionEntry ExtractUseRequestTimeoutConvention(AttributeData attribute) {
        var policyName = attribute.TryGetConstructorArguments(index: 0, out var output) && output is { Value: not null }
            ? (string)output.Value
            : null;

        return policyName is not null
            ? new ConventionEntry(
                Call: $"WithRequestTimeout(\"{EscapeStringLiteral(policyName)}\")"
            )
            : default;
    }
    
    private static ConventionEntry ExtractDisableValidationConvention() {
        return new ConventionEntry(
            Call: "DisableValidation()"
        );
    }

    private static ConventionEntry ExtractVersionConvention(AttributeData attribute) {
        var version = attribute.TryGetConstructorArguments(index: 0, out var versionConstant) && versionConstant is { Value: not null }
            ? versionConstant.Value.ToString()
            : null;

        if (VersionParser.TryParse(version ?? string.Empty, out var major, out var minor)) {
            return new ConventionEntry(
                Call: $"MapToApiVersion(new {NewFQN.ApiVersion}({major}, {minor}))"
            );
        }

        return default;
    }

    private static ConventionEntry ExtractDescriptionSummaryConvention(AttributeData attribute) {
        var name = attribute.GetNamedArgument("Name").GetValue<string?>();
        var description = attribute.GetNamedArgument("Description").GetValue<string?>();
        var summary = attribute.GetNamedArgument("Summary").GetValue<string?>();
        var tags = attribute.GetNamedArgument("Tags").GetValues<string>();

        var calls = new List<string>();

        if (!string.IsNullOrWhiteSpace(name)) {
            calls.Add($"WithName(\"{EscapeStringLiteral(name)}\")");
        }

        if (!string.IsNullOrWhiteSpace(description)) {
            calls.Add($"WithDescription(\"{EscapeStringLiteral(description)}\")");
        }

        if (!string.IsNullOrWhiteSpace(summary)) {
            calls.Add($"WithSummary(\"{EscapeStringLiteral(summary)}\")");
        }

        if (tags.Length > 0) {
            var values = tags.Select(static tag => $"\"{EscapeStringLiteral(tag)}\"");

            calls.Add($"WithTags({string.Join(", ", values)})");
        }

        return new ConventionEntry(
            Call: string.Join(".", calls)
        );
    }

    private static string GetFullyQualifiedName(INamedTypeSymbol symbol) {
        return symbol.OriginalDefinition.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
        );
    }
}