using Microsoft.CodeAnalysis;
using Nameless.Generators.Diagnostics;
using Nameless.Generators.Infrastructure;
using Nameless.Generators.Models;
using Nameless.Generators.Web.Http.Endpoints.Diagnostics;
using Nameless.Generators.Web.Http.Endpoints.Infrastructure;
using Nameless.Generators.Web.Http.Endpoints.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Conventions;

public static class ConventionExtractor {
    public static DiagnosticAwareResult<ConventionCollection> Extract(INamedTypeSymbol classSymbol, LocationModel location, CancellationToken cancellationToken) {
        var attributes = classSymbol.GetAttributes();

        if (attributes.IsDefaultOrEmpty) { return new ConventionCollection(); }

        var diagnostics = new List<GeneratorDiagnostic>();
        var result = new ConventionCollection();

        foreach (var attribute in attributes) {
            cancellationToken.ThrowIfCancellationRequested();

            if (TryExtractConvention(attribute, out var output)) {
                result.Add(output);

                continue;
            }

            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassUnknownAttribute,
                location: location,
                messageArgs: [classSymbol.Name, attribute.AttributeClass?.Name!]
            ));
        }

        return (result, [.. diagnostics]);
    }

    private static bool TryExtractConvention(AttributeData attribute, out Convention output) {
        output = attribute.GetAttributeDefinition() switch {
            AttributeDefinitions.Endpoint or AttributeDefinitions.EndpointGroup => ExtractEndpointOrEndpointGroupConvention(attribute),

            AttributeDefinitions.DisableAntiforgery => ExtractDisableAntiforgeryConvention(),

            AttributeDefinitions.AllowAnonymous => ExtractAllowAnonymousConvention(),
            AttributeDefinitions.Authorize => ExtractAuthorizeConvention(attribute),

            AttributeDefinitions.DisableCookieRedirect => ExtractDisableCookieRedirectConvention(),
            AttributeDefinitions.AllowCookieRedirect => ExtractAllowCookieRedirectConvention(),

            AttributeDefinitions.DisableCors => ExtractDisableCorsConvention(),
            AttributeDefinitions.EnableCors => ExtractEnableCorsConvention(attribute),

            AttributeDefinitions.UseFilter => ExtractUseFilterConvention(attribute),

            AttributeDefinitions.DisableHttpMetrics => ExtractDisableHttpMetricsConvention(),

            AttributeDefinitions.DisableOutputCache => ExtractDisableOutputCacheConvention(),
            AttributeDefinitions.OutputCache => ExtractOutputCacheConvention(attribute),

            AttributeDefinitions.ProducesResponse => ExtractProducesConvention(attribute),
            AttributeDefinitions.ProducesProblemResponse => ExtractProducesProblemConvention(attribute),
            AttributeDefinitions.ProducesValidationProblemResponse => ExtractProducesValidationProblemConvention(attribute),

            AttributeDefinitions.DisableRateLimiting => ExtractDisableRateLimitingConvention(),
            AttributeDefinitions.EnableRateLimiting => ExtractEnableRateLimitingConvention(attribute),

            AttributeDefinitions.DisableRequestTimeout => ExtractDisableRequestTimeoutConvention(),
            AttributeDefinitions.RequestTimeout => ExtractRequestTimeoutConvention(attribute),

            AttributeDefinitions.DisableValidation => ExtractDisableValidationConvention(),
            AttributeDefinitions.EnableValidation => ExtractEnableValidationConvention(),

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

    private static Convention ExtractAuthorizeConvention(AttributeData attribute) {
        var policyCtor = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var policyArg = attribute.GetNamedArgument("Policy").GetPrimitiveValue<string?>();
        var roles = attribute.GetNamedArgument("Roles").GetPrimitiveValue<string?>();
        var authenticationSchemes = attribute.GetNamedArgument("AuthenticationSchemes").GetPrimitiveValue<string?>();

        if (string.IsNullOrWhiteSpace(policyCtor) &&
            string.IsNullOrWhiteSpace(policyArg) &&
            string.IsNullOrWhiteSpace(roles) &&
            string.IsNullOrWhiteSpace(authenticationSchemes)) {
            return new Convention(
                call: ".RequireAuthorization()"
            );
        }

        var properties = new List<string>();

        var policy = policyCtor ?? policyArg;
        if (!string.IsNullOrWhiteSpace(policy)) {
            properties.Add($"Policy = \"{EscapeStringLiteral(policy)}\"");
        }

        if (!string.IsNullOrWhiteSpace(roles)) {
            properties.Add($"Roles = \"{EscapeStringLiteral(roles)}\"");
        }

        if (!string.IsNullOrWhiteSpace(authenticationSchemes)) {
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

    private static Convention ExtractEnableCorsConvention(AttributeData attribute) {
        var policyCtor = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        var policyArg = attribute.GetNamedArgument("PolicyName").GetPrimitiveValue<string?>();

        if (string.IsNullOrWhiteSpace(policyCtor) && string.IsNullOrWhiteSpace(policyArg)) {
            return new Convention(".RequireCors()");
        }

        return new Convention($".RequireCors(\"{EscapeStringLiteral(policyCtor ?? policyArg)}\")");
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

    private static Convention ExtractOutputCacheConvention(AttributeData attribute) {
        var duration = attribute.GetNamedArgument("Duration").GetPrimitiveValue<int?>();
        var noStore = attribute.GetNamedArgument("NoStore").GetPrimitiveValue<bool?>();
        var varyByQueryKeys = attribute.GetNamedArgument("VaryByQueryKeys").GetArrayValue<string>();
        var varyByHeaderNames = attribute.GetNamedArgument("VaryByHeaderNames").GetArrayValue<string>();
        var varyByRouteValueNames = attribute.GetNamedArgument("VaryByRouteValueNames").GetArrayValue<string>();
        var tags = attribute.GetNamedArgument("Tags").GetArrayValue<string>();
        var policyName = attribute.GetNamedArgument("PolicyName").GetPrimitiveValue<string?>();

        if (duration is null &&
            noStore is null &&
            varyByQueryKeys.Length == 0 &&
            varyByHeaderNames.Length == 0 &&
            varyByRouteValueNames.Length == 0 &&
            tags.Length == 0 &&
            string.IsNullOrWhiteSpace(policyName)) {
            return new Convention(".CacheOutput()");
        }

        if (noStore is true) {
            return new Convention(".CacheOutput(policy => policy.NoCache())");
        }

        if (!string.IsNullOrWhiteSpace(policyName)) {
            return new Convention($".CacheOutput(\"{EscapeStringLiteral(policyName)}\")");
        }

        var cw = new CodeWriter();
        using (cw.Block(".CacheOutput(policy => {", closing: "})")) {
            cw.WriteLine("policy.Cache();");

            if (duration.GetValueOrDefault() >= 0) {
                cw.WriteLine($"policy.Expire(TimeSpan.FromSeconds({duration}));");
            }

            string value;
            if (varyByQueryKeys.Length > 0) {
                value = string.Join(", ", varyByQueryKeys.Select<string, string>(item => $"\"{EscapeStringLiteral(item)}\""));
                cw.WriteLine($"policy.SetVaryByQuery({value});");
            }

            if (varyByHeaderNames.Length > 0) {
                value = string.Join(", ", varyByHeaderNames.Select<string, string>(item => $"\"{EscapeStringLiteral(item)}\""));
                cw.WriteLine($"policy.SetVaryByHeader({value});");

            }

            if (varyByRouteValueNames.Length > 0) {
                value = string.Join(", ", varyByRouteValueNames.Select<string, string>(item => $"\"{EscapeStringLiteral(item)}\""));
                cw.WriteLine($"policy.SetVaryByRouteValue({value});");
            }

            if (tags.Length > 0) {
                value = string.Join(", ", tags.Select<string, string>(item => $"\"{EscapeStringLiteral(item)}\""));
                cw.WriteLine($"policy.Tag({value});");
            }
        }

        return new Convention(call: cw.GetCode());
    }

    private static Convention ExtractProducesConvention(AttributeData attribute) {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) { return default; }

        var statusCode = GetProducesResponseStatusCode(attribute);
        var contentType = GetProducesResponseContentType(attribute);
        var additionalContentTypes = GetProducesResponseAdditionalContentTypes(attribute);

        // Generic usage [ProducesResponse<T>]: T is a type argument, not a ctor arg
        if (attributeClass.TryGetTypeArgument(index: 0, out var typeArg)) {
            return new Convention(
                call: $".Produces<{typeArg.GetFullyQualifiedName()}>({statusCode}, {contentType}, {additionalContentTypes})"
            );
        }

        // Generic usage [ProducesResponse(type: ...)]: 'type' is a constructor argument, not a type arg
        if (attribute.GetConstructorArgument(index: 0).GetSymbolValue() is { } ctorArg) {
            return new Convention(
                call: $".Produces(responseType: typeof({ctorArg.GetFullyQualifiedName()}), {statusCode}, {contentType}, {additionalContentTypes})"
            );
        }

        return default;
    }

    private static Convention ExtractProducesProblemConvention(AttributeData attribute) {
        var statusCode = GetProducesResponseStatusCode(attribute, fallback: 500);
        var contentType = GetProducesResponseContentType(attribute, fallback: ContentTypes.JsonProblem);

        return new Convention(
            call: $".ProducesProblem({statusCode}, {contentType})"
        );
    }

    private static Convention ExtractProducesValidationProblemConvention(AttributeData attribute) {
        var statusCode = GetProducesResponseStatusCode(attribute, fallback: 400);
        var contentType = GetProducesResponseContentType(attribute, fallback: ContentTypes.JsonProblem);

        return new Convention(
            call: $".ProducesValidationProblem({statusCode}, {contentType})"
        );
    }

    private static string GetProducesResponseStatusCode(AttributeData attribute, int fallback = 200) {
        var arg = attribute.GetNamedArgument("StatusCode").GetPrimitiveValue<int?>();

        return arg > 0 ? $"statusCode: {arg}" : $"statusCode: {fallback}";
    }

    private static string GetProducesResponseContentType(AttributeData attribute, string fallback = ContentTypes.Json) {
        var arg = attribute.GetNamedArgument("ContentType").GetPrimitiveValue<string?>();

        return !string.IsNullOrWhiteSpace(arg)
            ? $"contentType: \"{arg}\""
            : $"contentType: \"{fallback}\"";
    }

    private static string GetProducesResponseAdditionalContentTypes(AttributeData attribute) {
        var arg = attribute.GetNamedArgument("AdditionalContentTypes").GetArrayValue<string>();
        var value = string.Join(", ", arg.Select<string, string>(item => $"\"{EscapeStringLiteral(item)}\""));

        return $"additionalContentTypes: [{value}]";
    }

    private static Convention ExtractDisableRateLimitingConvention() {
        return new Convention(
            call: ".DisableRateLimiting()"
        );
    }

    private static Convention ExtractEnableRateLimitingConvention(AttributeData attribute) {
        var policyName = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();

        return !string.IsNullOrWhiteSpace(policyName)
            ? new Convention($".RequireRateLimiting(policyName: \"{EscapeStringLiteral(policyName)}\")")
            : default;
    }

    private static Convention ExtractDisableRequestTimeoutConvention() {
        return new Convention(
            call: ".DisableRequestTimeout()"
        );
    }

    private static Convention ExtractRequestTimeoutConvention(AttributeData attribute) {
        var arg = attribute.GetConstructorArgument(index: 0);

        return arg switch {
            { Kind: TypedConstantKind.Primitive, Value: int milliseconds and > 0 }
                => new Convention($".WithRequestTimeout(timeout: TimeSpan.FromMilliseconds({milliseconds}))"),

            { Kind: TypedConstantKind.Primitive, Value: string policyName }
                => new Convention($".WithRequestTimeout(policyName: \"{EscapeStringLiteral(policyName)}\")"),

            _ => default
        };
    }

    private static Convention ExtractDisableValidationConvention() {
        return new Convention(
            call: ".DisableValidation()"
        );
    }

    private static Convention ExtractEnableValidationConvention() {
        return new Convention(
            call: ".WithRequestValidation()"
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
            var values = tags.Select<string, string>(static tag => $"\"{EscapeStringLiteral(tag)}\"");

            cw.WriteLine($".WithTags({string.Join(", ", values)})");
        }

        WriteEndpointSpecificConventions(cw, attribute);

        return new Convention(
            call: cw.GetCode()
        );
    }

    private static void WriteEndpointSpecificConventions(CodeWriter cw, AttributeData attribute) {
        if (attribute.GetAttributeDefinition() != AttributeDefinitions.Endpoint) {
            return;
        }

        var name = attribute.GetNamedArgument("Name").GetPrimitiveValue<string?>();
        if (!string.IsNullOrWhiteSpace(name)) {
            cw.WriteLine($".WithName(\"{EscapeStringLiteral(name)}\")");
        }

        var value = attribute.GetNamedArgument("Version").GetPrimitiveValue<string?>();
        var version = string.IsNullOrWhiteSpace(value)
            ? VersionModel.V1
            : VersionModel.TryParse(value, out var output)
                ? output
                : default;

        if (version == default) {
            return;
        }

        var major = $"majorVersion: {version.Major}";
        var minor = $"minorVersion: {(version.Minor is not null ? version.Minor : "null")}";
        var status = $"status: {(version.Status is not null ? $"\"{EscapeStringLiteral(version.Status)}\"" : "null")}";

        cw.WriteLine($".MapToApiVersion(new ApiVersion({major}, {minor}, {status}))");
    }
}