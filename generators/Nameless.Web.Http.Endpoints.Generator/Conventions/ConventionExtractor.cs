using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Conventions;

public static class ConventionExtractor {
    public static ConventionCollection Extract(ImmutableArray<AttributeData> attributes, CancellationToken cancellationToken) {
        if (attributes.IsDefaultOrEmpty) { return []; }

        var result = new ConventionCollection();

        foreach (var attribute in attributes) {
            cancellationToken.ThrowIfCancellationRequested();

            if (TryExtractConvention(attribute, out var output)) {
                result.Add(output);
            }
        }

        return result;
    }

    private static bool TryExtractConvention(AttributeData attribute, out Convention output) {
        output = attribute.GetAttributeDefinition() switch {
            AttributeDefinitions.Endpoint or AttributeDefinitions.EndpointGroup => ExtractEndpointOrEndpointGroupConvention(attribute),

            AttributeDefinitions.DisableAntiforgery => ExtractDisableAntiforgeryConvention(),
            
            AttributeDefinitions.AllowAnonymous => ExtractAllowAnonymousConvention(),
            AttributeDefinitions.UseAuthorization => ExtractUseAuthorizationConvention(attribute),
            
            AttributeDefinitions.DisableCookieRedirect => ExtractDisableCookieRedirectConvention(),
            AttributeDefinitions.AllowCookieRedirect => ExtractAllowCookieRedirectConvention(),

            AttributeDefinitions.DisableCors => ExtractDisableCorsConvention(),
            AttributeDefinitions.UseCors => ExtractUseCorsConvention(attribute),

            AttributeDefinitions.UseFilter => ExtractUseFilterConvention(attribute),

            AttributeDefinitions.DisableHttpMetrics => ExtractDisableHttpMetricsConvention(),
            
            AttributeDefinitions.DisableOutputCache => ExtractDisableOutputCacheConvention(),
            AttributeDefinitions.UseOutputCache => ExtractUseOutputCacheConvention(attribute),
            
            AttributeDefinitions.Produces => ExtractProducesConvention(attribute),
            AttributeDefinitions.ProducesProblem => ExtractProducesProblemConvention(attribute),
            AttributeDefinitions.ProducesValidationProblem => ExtractProducesValidationProblemConvention(attribute),
            
            AttributeDefinitions.DisableRateLimiting => ExtractDisableRateLimitingConvention(),
            AttributeDefinitions.UseRateLimiting => ExtractUseRateLimitingConvention(attribute),
            
            AttributeDefinitions.DisableRequestTimeout => ExtractDisableRequestTimeoutConvention(),
            AttributeDefinitions.UseRequestTimeout => ExtractUseRequestTimeoutConvention(attribute),
            
            AttributeDefinitions.DisableValidation => ExtractDisableValidationConvention(),
            
            AttributeDefinitions.Deprecate => ExtractDeprecateConvention(attribute),

            _ => default
        };

        return !string.IsNullOrWhiteSpace(output.Call);
    }
    
    private static Convention ExtractDisableAntiforgeryConvention() {
        return new Convention(
            call: ".DisableAntiforgery()"
        );
    }

    private static Convention ExtractAllowAnonymousConvention() {
        return new Convention(
            call: ".AllowAnonymous()"
        );
    }

    private static Convention ExtractUseAuthorizationConvention(AttributeData attribute) {
        var policyName = attribute.GetNamedArgument("PolicyName").GetPrimitiveValue<string?>();
        var roles = attribute.GetNamedArgument("Roles").GetPrimitiveValue<string?>();
        var authenticationSchemes = attribute.GetNamedArgument("AuthenticationSchemes").GetPrimitiveValue<string?>();

        if (policyName is null && roles is null && authenticationSchemes is null) {
            return new Convention(
                call: ".RequireAuthorization()"
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

        return new Convention(
            call: $".RequireAuthorization(new AuthorizeAttribute {{ {string.Join(", ", properties)} }})"
        );
    }

    private static Convention ExtractAllowCookieRedirectConvention() {
        return new Convention(
            call: ".AllowCookieRedirect()"
        );
    }

    private static Convention ExtractDisableCookieRedirectConvention() {
        return new Convention(
            call: ".DisableCookieRedirect()"
        );
    }

    private static Convention ExtractDisableCorsConvention() {
        return new Convention(
            call: ".WithMetadata(new DisableCorsAttribute())"
        );
    }

    private static Convention ExtractUseCorsConvention(AttributeData attribute) {
        var policyName = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var call = $".RequireCors(\"{EscapeStringLiteral(policyName)}\")";

        return !string.IsNullOrWhiteSpace(policyName) ? new Convention(call) : default;
    }

    private static Convention ExtractUseFilterConvention(AttributeData attribute) {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return default; }

        string? filter = null;

        // Generic usage [UseFilter<T>]: T is a type argument, not a ctor arg
        if (attributeClass.TryGetTypeArgument(index: 0, out var typeArg)) {
            filter = typeArg.GetFullyQualifiedName();
        }

        // Usage [UseFilter(filterType: typeof(T))]: argument is a ctor arg
        if (attribute.GetConstructorArgument(index: 0).GetSymbolValue() is { } ctorArg) {
            filter = ctorArg.GetFullyQualifiedName();
        }

        return filter is not null
            ? new Convention(call: $".AddEndpointFilter<{filter}>()")
            : default;
    }

    private static Convention ExtractDisableHttpMetricsConvention() {
        return new Convention(
            call: ".DisableHttpMetrics()"
        );
    }

    private static Convention ExtractDisableOutputCacheConvention() {
        return new Convention(
            call: ".CacheOutput(policy => policy.NoCache())"
        );
    }

    private static Convention ExtractUseOutputCacheConvention(AttributeData attribute) {
        var policyName = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var call = $".CacheOutput(\"{EscapeStringLiteral(policyName)}\")";

        return !string.IsNullOrWhiteSpace(policyName) ? new Convention(call) : default;
    }

    private static Convention ExtractProducesConvention(AttributeData attribute) {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return default; }

        var responseType = "object";
        var statusCode = 200;
        var contentType = ContentTypes.Json;

        // Generic usage [Produces<T>]: T is a type argument, not a ctor arg
        if (attributeClass.TryGetTypeArgument(index: 0, out var typeArg)) {
            responseType = typeArg.GetFullyQualifiedName();
            statusCode = GetStatusCode(attribute, index: 0);
            contentType = GetContentType(attribute, index: 1);
        }
        else {
            if (attribute.GetConstructorArgument(index: 0).GetSymbolValue() is { } ctorArg) {
                responseType = ctorArg.GetFullyQualifiedName();
            }

            statusCode = GetStatusCode(attribute, index: 1);
            contentType = GetContentType(attribute, index: 2);
        }

        return new Convention(
            call: $".Produces<{responseType}>(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static Convention ExtractProducesProblemConvention(AttributeData attribute) {
        var statusCode = GetStatusCode(attribute, index: 0, fallback: 500);
        var contentType = GetContentType(attribute, index: 1, fallback: ContentTypes.JsonProblem);

        return new Convention(
            call: $".ProducesProblem(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static Convention ExtractProducesValidationProblemConvention(AttributeData attribute) {
        var statusCode = GetStatusCode(attribute, index: 0, fallback: 400);
        var contentType = GetContentType(attribute, index: 1, fallback: ContentTypes.JsonProblem);

        return new Convention(
            call: $".ProducesValidationProblem(statusCode: {statusCode}, contentType: \"{contentType}\")"
        );
    }

    private static int GetStatusCode(AttributeData attribute, int index, int fallback = 200) {
        return attribute.TryGetConstructorArgument(index, out var output) && output is { Value: int statusCode }
            ? statusCode
            : fallback;
    }

    private static string GetContentType(AttributeData attribute, int index, string fallback = ContentTypes.Json) {
        return attribute.TryGetConstructorArgument(index, out var output) && output is { Value: string contentType } && !string.IsNullOrWhiteSpace(contentType)
            ? contentType
            : fallback;
    }

    private static Convention ExtractDisableRateLimitingConvention() {
        return new Convention(
            call: ".DisableRateLimiting()"
        );
    }

    private static Convention ExtractUseRateLimitingConvention(AttributeData attribute) {
        var policyName = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var call = $".RequireRateLimiting(\"{EscapeStringLiteral(policyName)}\")";

        return !string.IsNullOrWhiteSpace(policyName) ? new Convention(call) : default;
    }

    private static Convention ExtractDisableRequestTimeoutConvention() {
        return new Convention(
            call: ".DisableRequestTimeout()"
        );
    }

    private static Convention ExtractUseRequestTimeoutConvention(AttributeData attribute) {
        var policyName = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var call = $".WithRequestTimeout(\"{EscapeStringLiteral(policyName)}\")";

        return !string.IsNullOrWhiteSpace(policyName) ? new Convention(call) : default;
    }

    private static Convention ExtractDisableValidationConvention() {
        return new Convention(
            call: ".DisableValidation()"
        );
    }

    private static Convention ExtractDeprecateConvention(AttributeData attribute) {
        var message = attribute.GetNamedArgument("Message").GetPrimitiveValue<string?>();

        var cw = new CodeWriter();

        using (cw.Block(".AddOpenApiOperationTransformer((op, _, _) => {", closing: "})")) {
            cw.WriteLine("op.Deprecated = true;");
            
            if (!string.IsNullOrWhiteSpace(message)) {
                cw.WriteLine($"op.Description = \"{EscapeStringLiteral(message)}\";");
            }

            cw.WriteLine("op.Extensions ??= new Dictionary<string, IOpenApiExtension>();");
            cw.WriteLine("op.Extensions.TryAdd(\"x-scalar-stability\", new JsonNodeExtension(\"deprecated\"));");
            cw.WriteLine();
            cw.WriteLine("return Task.CompletedTask;");
        }

        var sunset = attribute.GetNamedArgument("Sunset").GetPrimitiveValue<string?>();
        var link = attribute.GetNamedArgument("Link").GetPrimitiveValue<string?>();
        if (!string.IsNullOrWhiteSpace(sunset)) {
            cw.Write(".WithSunset(");
            cw.Write($"DateTimeOffset.ParseExact(\"{EscapeStringLiteral(sunset)}\", \"R\", CultureInfo.InvariantCulture)");
            
            if (!string.IsNullOrWhiteSpace(link)) {
                cw.Write($", link: \"{link}\"");
            }

            cw.WriteLine(")");
        }

        return new Convention(
            call: cw.GetCode()
        );
    }

    private static Convention ExtractEndpointOrEndpointGroupConvention(AttributeData attribute) {
        if (attribute.AttributeClass is null) { return default; }

        var cw = new CodeWriter();

        var attributeDefinition = attribute.GetAttributeDefinition();
        var name = attributeDefinition switch {
            AttributeDefinitions.Endpoint => attribute.GetNamedArgument("Name").GetPrimitiveValue<string?>(),
            AttributeDefinitions.EndpointGroup => attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>(),
            _ => null
        };
        if (!string.IsNullOrWhiteSpace(name)) {
            cw.WriteLine($".WithName(\"{EscapeStringLiteral(name)}\")");
        }

        var description = attribute.GetNamedArgument("Description").GetPrimitiveValue<string?>();
        if (!string.IsNullOrWhiteSpace(description)) {
            cw.WriteLine($".WithDescription(\"{EscapeStringLiteral(description)}\")");
        }

        var summary = attribute.GetNamedArgument("Summary").GetPrimitiveValue<string?>();
        if (!string.IsNullOrWhiteSpace(summary)) {
            cw.WriteLine($".WithSummary(\"{EscapeStringLiteral(summary)}\")");
        }

        var tags = attribute.GetNamedArgument("Tags").GetArrayValue<string>();
        if (tags.Length > 0) {
            var values = tags.Select(static tag => $"\"{EscapeStringLiteral(tag)}\"");

            cw.WriteLine($".WithTags({string.Join(", ", values)})");
        }

        if (attributeDefinition == AttributeDefinitions.Endpoint) {
            var value = attribute.GetNamedArgument("Version").GetPrimitiveValue<string?>();
            var version = string.IsNullOrWhiteSpace(value)
                ? VersionModel.V1
                : VersionModel.TryParse(value, out var output)
                    ? output
                    : default;

            if (version != default) {
                var major = $"majorVersion: {version.Major}";
                var minor = $"minorVersion: {(version.Minor is not null ? version.Minor : "null")}";
                var status = $"status: {(version.Status is not null ? $"\"{EscapeStringLiteral(version.Status)}\"" : "null")}";

                cw.WriteLine($".MapToApiVersion(new ApiVersion({major}, {minor}, {status}))");
            }
        }

        return new Convention(
            call: cw.GetCode()
        );
    }
}