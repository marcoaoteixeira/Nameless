using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class SemanticExtractor {
    internal static ExtractionResult Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return ExtractionResult.Failure([]);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var location = context.TargetNode.GetLocation();
        var filePath = location.SourceTree?.FilePath ?? string.Empty;
        var lineSpan = location.GetLineSpan();
        var line = lineSpan.StartLinePosition.Line;
        var character = lineSpan.StartLinePosition.Character;

        var classDeclaration = context.TargetNode as ClassDeclarationSyntax;
        var isPartial = classDeclaration?.Modifiers.Any(SyntaxKind.PartialKeyword) ?? false;

        if (!isPartial) {
            return ExtractionResult.Failure([
                new GeneratorDiagnostic(
                    Descriptor: DiagnosticDescriptors.ClassMustBePartial,
                    FilePath: filePath, StartLine: line, StartCharacter: character,
                    MessageArgs: [classSymbol.Name])
            ]);
        }
         
        var endpointAttr = classSymbol.GetEndpointAttribute();
        if (endpointAttr is null) {
            return ExtractionResult.Failure([]);
        }

        var diagnostics = ImmutableArray.CreateBuilder<GeneratorDiagnostic>();

        var endpointMetadata = ExtractEndpointMetadata(endpointAttr);

        var endpointExecutionHandle = GetEndpointExecutionHandle(classSymbol);
        if (endpointExecutionHandle is null) {
            diagnostics.Add(new GeneratorDiagnostic(
                Descriptor: DiagnosticDescriptors.MissingEndpointExecutionHandle,
                FilePath: filePath, StartLine: line, StartCharacter: character,
                MessageArgs: [classSymbol.Name]
            ));

            return ExtractionResult.Failure(diagnostics.ToImmutable());
        }

        if (endpointExecutionHandle.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal) {
            diagnostics.Add(new GeneratorDiagnostic(
                Descriptor: DiagnosticDescriptors.EndpointExecutionHandleNotAccessible,
                FilePath: filePath, StartLine: line, StartCharacter: character,
                MessageArgs: [classSymbol.Name]
            ));
        }

        var parameters = ExtractParameters(endpointExecutionHandle, cancellationToken);
        var versionMetadata = ExtractVersionMetadata(classSymbol, filePath, line, character);

        diagnostics.AddRange(versionMetadata.Diagnostics);

        var produces = ExtractProduces(classSymbol);
        var filters = classSymbol.GetFilterTypeNames();

        var groupTypeArg = endpointAttr.GetArg("Group");
        var groupTypeFqn = groupTypeArg is { Kind: TypedConstantKind.Type, Value: ITypeSymbol groupTypeSymbol }
            ? groupTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            : null;

        var acceptsAttr = classSymbol.GetAcceptsAttribute();
        var summary = classSymbol.GetEndpointSummaryAttribute().GetCtorArgValue<string?>(index: 0);
        var description = classSymbol.GetEndpointDescriptionAttribute().GetCtorArgValue<string?>(index: 0);
        var authorizeAttr = classSymbol.GetAuthorizeAttribute();
        var allowAnonAttr = classSymbol.GetAllowsAnonymousAttribute();
        var corsAttr = classSymbol.GetEnableCorsAttribute();
        var rateLimitAttr = classSymbol.GetEnableRateLimitingAttribute();
        var outputCacheAttr = classSymbol.GetOutputCacheAttribute();
        var requestTimeoutAttr = classSymbol.GetRequestTimeoutAttribute();
        var disableMetricsAttr = classSymbol.GetDisableHttpMetricsAttribute();
        var antiforgeryAttr = classSymbol.GetRequireAntiforgeryTokenAttribute();

        if (authorizeAttr is not null && allowAnonAttr is not null) {
            diagnostics.Add(new GeneratorDiagnostic(
                Descriptor: DiagnosticDescriptors.ConflictingAuthAttributes,
                FilePath: filePath, StartLine: line, StartCharacter: character,
                MessageArgs: [classSymbol.Name]
            ));
        }

        var accessModifier = classSymbol.DeclaredAccessibility switch {
            Accessibility.Public => "public",
            _ => "internal"
        };

        bool? requireAntiforgery = antiforgeryAttr is not null
            ? (antiforgeryAttr.GetCtorArgValue<bool?>(index: 0) ?? true)
            : null;

        var model = new EndpointModel(
            ClassName: classSymbol.Name,
            Namespace: classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToDisplayString(),
            AccessModifier: accessModifier,
            Metadata: endpointMetadata,
            GroupTypeFqn: groupTypeFqn,
            Versions: versionMetadata.Versions,
            Parameters: parameters,
            Produces: produces,
            FilterTypeNames: filters,
            AcceptsTypeName: acceptsAttr.GetTypeArg(),
            AcceptsContentType: acceptsAttr.GetCtorArgValue<string?>(index: 0) ?? acceptsAttr.GetPropValue<string?>("ContentType"),
            RequireAntiforgery: requireAntiforgery,
            Summary: summary,
            Description: description,
            RequiresAuthorization: authorizeAttr is not null,
            AuthorizationPolicy: authorizeAttr.GetCtorArgValue<string?>(index: 0),
            AllowAnonymous: allowAnonAttr is not null,
            CorsPolicy: corsAttr.GetCtorArgValue<string?>(index: 0),
            RateLimitingPolicy: rateLimitAttr.GetCtorArgValue<string?>(index: 0),
            OutputCachePolicy: outputCacheAttr.GetPropValue<string?>("PolicyName"),
            RequestTimeoutPolicy: requestTimeoutAttr.GetCtorArgValue<string?>(index: 0),
            DisableHttpMetrics: disableMetricsAttr is not null,

            FilePath: filePath,
            StartLine: line,
            StartCharacter: character);

        return ExtractionResult.Success(model, diagnostics.ToImmutable());
    }

    private static EndpointMetadata ExtractEndpointMetadata(AttributeData attributeData) {
        if (attributeData.AttributeClass is null) {
            return new EndpointMetadata(HttpVerbs.GET, string.Empty, null, []);
        }

        var httpVerb = attributeData.AttributeClass.TypeArguments.Length > 0
            ? attributeData.AttributeClass.TypeArguments[0].ToDisplayString()
            : string.Empty;

        var method = httpVerb switch {
            FQN.GET_HTTP_VERB => HttpVerbs.GET,
            FQN.POST_HTTP_VERB => HttpVerbs.POST,
            FQN.PUT_HTTP_VERB => HttpVerbs.PUT,
            FQN.PATCH_HTTP_VERB => HttpVerbs.PATCH,
            FQN.DELETE_HTTP_VERB => HttpVerbs.DELETE,
            FQN.HEAD_HTTP_VERB => HttpVerbs.HEADER,
            FQN.OPTIONS_HTTP_VERB => HttpVerbs.OPTIONS,
            _ => HttpVerbs.GET
        };

        var route = attributeData.ConstructorArguments.Length > 0
            && attributeData.ConstructorArguments[0].Value is string routeValue
            ? routeValue
            : string.Empty;

        var name = attributeData.NamedArguments
            .FirstOrDefault(static arg => arg.Key == "Name").Value.Value as string;

        var tagsConstant = attributeData.NamedArguments
            .FirstOrDefault(static arg => arg.Key == "Tags").Value;

        ImmutableArray<string> tags = tagsConstant.Kind == TypedConstantKind.Array
            ? [.. tagsConstant.Values.Select(static constant => constant.Value as string ?? string.Empty)]
            : [];

        return new EndpointMetadata(method, route, name, tags);
    }

    private static IMethodSymbol? GetEndpointExecutionHandle(INamedTypeSymbol classSymbol) {
        foreach (var member in classSymbol.GetMembers(ENDPOINT_EXECUTION_HANDLE_NAME)) {
            if (member is IMethodSymbol method) {
                return method;
            }
        }

        return null;
    }

    private static ImmutableArray<ParameterModel> ExtractParameters(IMethodSymbol endpointExecutionHandle, CancellationToken cancellationToken) {
        var result = ImmutableArray.CreateBuilder<ParameterModel>(endpointExecutionHandle.Parameters.Length);

        foreach (var parameter in endpointExecutionHandle.Parameters) {
            cancellationToken.ThrowIfCancellationRequested();

            var parameterTypeFqn = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var isCancellationToken = parameter.Type.ToDisplayString() == FQN.CANCELLATION_TOKEN;

            var bindingKind = ParameterBindingKind.None;
            string? bindingName = null;

            foreach (var attr in parameter.GetAttributes()) {
                var candidate = attr.AttributeClass?.ToDisplayString() switch {
                    FQN.FROM_BODY_ATTRIBUTE => ParameterBindingKind.FromBody,
                    FQN.FROM_ROUTE_ATTRIBUTE => ParameterBindingKind.FromRoute,
                    FQN.FROM_QUERY_ATTRIBUTE => ParameterBindingKind.FromQuery,
                    FQN.FROM_HEADER_ATTRIBUTE => ParameterBindingKind.FromHeader,
                    FQN.AS_PARAMETERS_ATTRIBUTE => ParameterBindingKind.AsParameters,
                    _ => ParameterBindingKind.None
                };

                if (candidate == ParameterBindingKind.None) { continue; }

                bindingKind = candidate;
                bindingName = attr.NamedArguments.FirstOrDefault(static arg => arg.Key == "Name").Value.Value as string;
                break;
            }

            result.Add(new ParameterModel(
                Name: parameter.Name,
                FullTypeName: parameterTypeFqn,
                IsCancellationToken: isCancellationToken,
                BindingKind: bindingKind,
                BindingName: bindingName));
        }
        return result.ToImmutable();
    }

    private static VersionMetadata ExtractVersionMetadata(INamedTypeSymbol classSymbol, string filePath, int line, int character) {
        var versions = ImmutableArray.CreateBuilder<VersionModel>();
        var diagnostics = ImmutableArray.CreateBuilder<GeneratorDiagnostic>();

        foreach (var attr in classSymbol.GetAttributes()) {
            if (attr.AttributeClass?.ToDisplayString() != FQN.VERSION_ATTRIBUTE) {
                continue;
            }

            if (attr.ConstructorArguments.Length == 0 || attr.ConstructorArguments[0].Value is not string versionValue) {
                continue;
            }

            if (VersionParser.TryParse(versionValue, out var major, out var minor, out var patch)) {
                var deprecated = attr.NamedArguments.FirstOrDefault(
                    static arg => arg.Key == "Deprecated"
                ).Value.Value is true;

                versions.Add(new VersionModel(major, minor, patch, deprecated));

                continue;
            }

            diagnostics.Add(new GeneratorDiagnostic(
                Descriptor: DiagnosticDescriptors.InvalidVersionString,
                FilePath: filePath,
                StartLine: line,
                StartCharacter: character,
                MessageArgs: [versionValue, classSymbol.Name]
            ));
        }

        return new VersionMetadata([.. versions], [.. diagnostics]);
    }

    private static ImmutableArray<ProducesModel> ExtractProduces(INamedTypeSymbol classSymbol) {
        var result = ImmutableArray.CreateBuilder<ProducesModel>();

        foreach (var attr in classSymbol.GetAttributes()) {
            if (TryExtractProduces(attr, out var output) && output is not null) {
                result.Add(output);

                continue;
            }

            if (TryExtractProducesProblem(attr, out output) && output is not null) {
                result.Add(output);

                continue;
            }

            if (TryExtractProducesValidationProblem(attr, out output) && output is not null) {
                result.Add(output);
            }
        }

        return [.. result];
    }

    private static bool TryExtractProduces(AttributeData attributeData, out ProducesModel? output) {
        output = null;

        var attrClass = attributeData.AttributeClass;
        if (attrClass is null) { return false; }

        if (!attrClass.IsAssignableTo(FQN.PRODUCES_ATTRIBUTE)) {
            return false;
        }

        string typeFqn;
        int statusCode;

        if (attrClass.TypeArguments.Length > 0) {
            typeFqn = attrClass.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            statusCode = attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is int statusCodeValue
                ? statusCodeValue
                : 200;
        }
        else {
            typeFqn = attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol typeArg
                ? typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                : FQN.OBJECT;
            statusCode = attributeData.ConstructorArguments.Length > 1 && attributeData.ConstructorArguments[1].Value is int statusCodeValue
                ? statusCodeValue
                : 200;
        }

        var contentType = attributeData.NamedArguments.FirstOrDefault(
            static constant => constant.Key == "ContentType"
        ).Value.Value as string;

        output = new ProducesModel(typeFqn, statusCode, contentType, ProducesKind.Response);

        return true;
    }

    private static bool TryExtractProducesProblem(AttributeData attributeData, out ProducesModel? output) {
        output = null;

        var attrClass = attributeData.AttributeClass;
        if (attrClass is null) { return false; }

        if (attrClass.ToDisplayString() != FQN.PRODUCES_PROBLEM_ATTRIBUTE) { return false; }

        var statusCode = attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is int statusCodeValue
            ? statusCodeValue
            : 500;

        var contentType = attributeData.NamedArguments.FirstOrDefault(
            static constant => constant.Key == "ContentType"
        ).Value.Value as string;

        output = new ProducesModel(string.Empty, statusCode, contentType, ProducesKind.Problem);

        return true;
    }

    private static bool TryExtractProducesValidationProblem(AttributeData attributeData, out ProducesModel? output) {
        output = null;

        var attrClass = attributeData.AttributeClass;
        if (attrClass is null) { return false; }

        if (attrClass.ToDisplayString() != FQN.PRODUCES_VALIDATION_PROBLEM_ATTRIBUTE) { return false; }

        var statusCode = attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is int statusCodeValue
            ? statusCodeValue
            : 400;

        var contentType = attributeData.NamedArguments.FirstOrDefault(
            static constant => constant.Key == "ContentType"
        ).Value.Value as string;

        output = new ProducesModel(string.Empty, statusCode, contentType, ProducesKind.ValidationProblem);

        return true;
    }
}
