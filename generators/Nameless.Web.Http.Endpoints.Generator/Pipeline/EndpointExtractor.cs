using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Models;
using Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class EndpointExtractor {
    internal static EndpointExtractionResult Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        var location = context.TargetNode.GetLocation();

        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return EndpointExtractionResult.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.WrongTargetSymbol,
                location: location,
                messageArgs: [context.TargetSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var classDeclarationSyntax = context.TargetNode as ClassDeclarationSyntax;
        var isPartial = classDeclarationSyntax?.Modifiers.Any(SyntaxKind.PartialKeyword) ?? false;

        if (!isPartial) {
            return EndpointExtractionResult.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassMustBePartial,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }
        
        var endpointAttribute = classSymbol.GetEndpointAttribute();
        if (endpointAttribute is null) {
            return EndpointExtractionResult.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.MissingEndpointAttribute,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        var endpointAttributeMetadata = ExtractEndpointAttributeMetadata(endpointAttribute);
        var endpointExecutionHandlerMetadata = ExtractEndpointExecutionHandlerMetadata(
            classSymbol,
            location,
            cancellationToken
        );
        if (!endpointExecutionHandlerMetadata.Diagnostics.IsEmpty) {
            return EndpointExtractionResult.Failure(endpointExecutionHandlerMetadata.Diagnostics);
        }

        var versionMetadata = ExtractEndpointVersionMetadata(classSymbol, location);
        if (!versionMetadata.Diagnostics.IsEmpty) {
            return EndpointExtractionResult.Failure(versionMetadata.Diagnostics);
        }
        
        var useAuthorizationAttr = classSymbol.GetUseAuthorizationAttribute();
        var allowsAnonymousAttr = classSymbol.GetAllowAnonymousAttribute();
        if (useAuthorizationAttr is not null && allowsAnonymousAttr is not null) {
            return EndpointExtractionResult.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingAuthAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        var classAccessModifier = GetEndpointClassAccessModifier(classSymbol);
        if (classAccessModifier is null) {
            return EndpointExtractionResult.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassMustBePublicOrInternal,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        var conventions = ConventionExtractor.Extract(classSymbol.GetAttributes());

        var model = new EndpointModel(
            Namespace: classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToDisplayString(),
            ClassName: classSymbol.Name,

            ClassAccessModifier: classAccessModifier,

            HttpVerb: endpointAttributeMetadata.HttpVerb,
            Route: endpointAttributeMetadata.Route,
            Group: endpointAttributeMetadata.Group,

            Handler: endpointExecutionHandlerMetadata.Handler,
            HandlerParameters: endpointExecutionHandlerMetadata.Parameters,

            Versions: versionMetadata.Versions,

            Conventions: conventions,

            FilePath: location.SourceTree?.FilePath ?? string.Empty,
            StartLine: location.GetLineSpan().StartLinePosition.Line,
            StartCharacter: location.GetLineSpan().StartLinePosition.Character
        );

        return EndpointExtractionResult.Success(model);
    }

    private static EndpointMetadata ExtractEndpointAttributeMetadata(AttributeData attributeData) {
        if (attributeData.AttributeClass is null) {
            return EndpointMetadata.Default;
        }

        var httpVerbFullyQualifiedName = attributeData.AttributeClass.GetTypeArgument(index: 0)?.ToDisplayString() ?? string.Empty;
        var httpVerb = httpVerbFullyQualifiedName switch {
            FQN.GET_HTTP_VERB => HttpVerbs.GET,
            FQN.POST_HTTP_VERB => HttpVerbs.POST,
            FQN.PUT_HTTP_VERB => HttpVerbs.PUT,
            FQN.PATCH_HTTP_VERB => HttpVerbs.PATCH,
            FQN.DELETE_HTTP_VERB => HttpVerbs.DELETE,
            FQN.HEAD_HTTP_VERB => HttpVerbs.HEADER,
            FQN.OPTIONS_HTTP_VERB => HttpVerbs.OPTIONS,
            _ => HttpVerbs.GET
        };

        var route = attributeData.GetConstructorArguments(index: 0).GetValue(fallback: string.Empty);
        var group = attributeData.GetNamedArgument("Group").GetFullyQualifiedName();

        return new EndpointMetadata(httpVerb, route, group);
    }

    private static EndpointExecutionHandlerMetadata ExtractEndpointExecutionHandlerMetadata(INamedTypeSymbol classSymbol, Location location, CancellationToken cancellationToken) {
        if (classSymbol.GetMembers(ENDPOINT_EXECUTION_HANDLER_NAME).SingleOrDefault(member => member is IMethodSymbol) is not IMethodSymbol handler) {
            return EndpointExecutionHandlerMetadata.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.MissingEndpointExecutionHandler,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        if (handler.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal) {
            return EndpointExecutionHandlerMetadata.Failure(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.EndpointExecutionHandlerNotAccessible,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        var parameters = ImmutableArray.CreateBuilder<HandlerParameterMetadata>(handler.Parameters.Length);

        foreach (var parameter in handler.Parameters) {
            cancellationToken.ThrowIfCancellationRequested();

            var parameterTypeFullyQualifiedName = parameter.Type.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            );
            var isCancellationToken = parameter.Type.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            ) == NewFQN.CancellationToken;

            var bindingKind = ParameterBindingKind.None;
            string? bindingName = null;

            foreach (var parameterAttribute in parameter.GetAttributes()) {
                var candidate = parameterAttribute.AttributeClass?.ToDisplayString() switch {
                    NewFQN.AsParametersAttribute => ParameterBindingKind.AsParameters,
                    NewFQN.FromBodyAttribute => ParameterBindingKind.FromBody,
                    NewFQN.FromFormAttribute => ParameterBindingKind.FromForm,
                    NewFQN.FromHeaderAttribute => ParameterBindingKind.FromHeader,
                    NewFQN.FromQueryAttribute => ParameterBindingKind.FromQuery,
                    NewFQN.FromRouteAttribute => ParameterBindingKind.FromRoute,
                    _ => ParameterBindingKind.None
                };

                if (candidate == ParameterBindingKind.None) { continue; }

                bindingKind = candidate;
                bindingName = parameterAttribute.GetNamedArgument("Name").GetValue<string?>();

                break;
            }

            parameters.Add(new HandlerParameterMetadata(
                Name: parameter.Name,
                FullTypeName: parameterTypeFullyQualifiedName,
                IsCancellationToken: isCancellationToken,
                BindingKind: bindingKind,
                BindingName: bindingName
            ));
        }

        return EndpointExecutionHandlerMetadata.Success(handler, [.. parameters]);
    }

    private static VersionMetadata ExtractEndpointVersionMetadata(INamedTypeSymbol classSymbol, Location location) {
        var versionAttribute = classSymbol.GetVersionAttribute();
        if (versionAttribute is null) { return VersionMetadata.Empty; }

        var value = versionAttribute.GetConstructorArguments(index: 0).Value?.ToString() ?? string.Empty;
        
        if (!VersionParser.TryParse(value, out var major, out var minor)) {
            return new VersionMetadata(Versions: [], Diagnostics: [
                GeneratorDiagnostic.Create(
                    descriptor: DiagnosticDescriptors.InvalidVersionString,
                    location: location,
                    messageArgs: [value, classSymbol.Name]
                )
            ]);
        }

        var deprecated = versionAttribute.GetNamedArgument("Deprecated").Value is true;

        return new VersionMetadata([
            new VersionModel(major, minor, Patch: 0, deprecated)
        ], Diagnostics: []);
    }

    private static string? GetEndpointClassAccessModifier(INamedTypeSymbol classSymbol) {
        return classSymbol.DeclaredAccessibility switch {
            Accessibility.Public => nameof(Accessibility.Public).ToLowerInvariant(),
            Accessibility.Internal => nameof(Accessibility.Internal).ToLowerInvariant(),
            _ => null
        };
    }
}
