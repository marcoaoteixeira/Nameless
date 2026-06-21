using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Generators.Conventions;
using Nameless.Web.Generators.Diagnostics;
using Nameless.Web.Generators.Infrastructure;
using Nameless.Web.Generators.Models;

namespace Nameless.Web.Generators.Pipeline;

public static class EndpointExtractor {
    public static DiagnosticAwareResult<EndpointModel> Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var location = new LocationModel(context.TargetNode.GetLocation());

        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.InvalidContextTargetSymbol,
                location: location,
                messageArgs: [context.TargetSymbol.Name]
            );
        }

        if (classSymbol.HasEndpointGroupAttribute()) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingEndpointVsEndpointGroupAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        var classDeclarationSyntax = context.TargetNode as ClassDeclarationSyntax;
        var classModelExtraction = ClassHelper.ExtractClassModel(
            classDeclarationSyntax!,
            classSymbol,
            location,
            cancellationToken
        );
        if (!classModelExtraction.Successful) {
            return classModelExtraction.Diagnostics;
        }

        var endpointAttribute = classSymbol.GetEndpointAttribute();
        if (endpointAttribute is null) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassMissingEndpointAttribute,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        var endpointArgumentsModelExtraction = ExtractEndpointArgumentsModel(
            endpointAttribute,
            classSymbol,
            location
        );
        if (!endpointArgumentsModelExtraction.Successful) {
            return endpointArgumentsModelExtraction.Diagnostics;
        }

        var endpointHandlerModelExtraction = ExtractEndpointHandlerModel(
            classSymbol,
            location,
            cancellationToken
        );
        if (!endpointHandlerModelExtraction.Successful) {
            return endpointHandlerModelExtraction.Diagnostics;
        }

        var conflicts = ClassHelper.ExtractAttributeConflicts(
            classSymbol,
            location,
            cancellationToken
        );

        var conventions = ConventionExtractor.Extract(
            classSymbol,
            location,
            cancellationToken
        );

        var model = new EndpointModel {
            Class = classModelExtraction.Model ?? throw new InvalidOperationException("Class model extraction failed."),
            Arguments = endpointArgumentsModelExtraction.Model ?? throw new InvalidOperationException("Endpoint arguments model extraction failed."),
            Conventions = conventions.Model ?? throw new InvalidOperationException("Convention model extraction failed."),
            Handler = endpointHandlerModelExtraction.Model ?? throw new InvalidOperationException("Endpoint handler model extraction failed."),
            Location = location
        };

        GeneratorDiagnostic[] diagnostics = [
            .. conflicts.Diagnostics,
            .. conventions.Diagnostics
        ];

        return (model, diagnostics);
    }

    private static DiagnosticAwareResult<EndpointArgumentsModel> ExtractEndpointArgumentsModel(AttributeData attributeData, INamedTypeSymbol classSymbol, LocationModel location) {
        if (attributeData.AttributeClass is null) {
            return new EndpointArgumentsModel();
        }

        var httpVerb = attributeData.GetNamedArgument("Verb").GetEnumValue<HttpVerbs>();
        var route = attributeData.GetConstructorArgument(index: 0).GetPrimitiveValue(fallback: string.Empty);
        var group = attributeData.GetNamedArgument("Group").GetSymbolValue()?.GetFullName() ?? string.Empty;
        var version = attributeData.GetNamedArgument("Version").GetPrimitiveValue<string?>();

        if (version is null) {
            return new EndpointArgumentsModel {
                HttpVerb = httpVerb,
                Route = route,
                Group = group,
                Version = VersionModel.V1
            };
        }

        if (VersionModel.TryParse(version, out var output)) {
            return new EndpointArgumentsModel {
                HttpVerb = httpVerb,
                Route = route,
                Group = group,
                Version = output
            };
        }

        // Has version but was impossible to parse
        return GeneratorDiagnostic.Create(
            descriptor: DiagnosticDescriptors.EndpointAttributeVersionArgumentIsInvalid,
            location: location,
            messageArgs: [classSymbol.Name, version]
        );
    }

    private static DiagnosticAwareResult<EndpointHandlerModel> ExtractEndpointHandlerModel(INamedTypeSymbol classSymbol, LocationModel location, CancellationToken cancellationToken) {
        var methodSymbol = classSymbol.GetMembers(EndpointClass.HandlerMethodName)
                                      .SingleOrDefault(member => member is IMethodSymbol);

        if (methodSymbol is not IMethodSymbol handler) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassMustDeclareHandlerMethod,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        if (handler.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassHandlerMethodMustBePublicOrInternal,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        var parameters = new List<EndpointHandlerParameterModel>(
            handler.Parameters.Length
        );

        foreach (var parameter in handler.Parameters) {
            cancellationToken.ThrowIfCancellationRequested();

            var type = parameter.Type.GetFullyQualifiedName();
            var isCancellationToken = type == FQN.CancellationToken;

            var bindingKind = ParameterBindingKind.None;
            string? bindingName = null;

            foreach (var parameterAttribute in parameter.GetAttributes()) {
                var candidate = parameterAttribute.AttributeClass?.GetFullyQualifiedName() switch {
                    FQN.AsParametersAttribute => ParameterBindingKind.AsParameters,
                    FQN.FromBodyAttribute => ParameterBindingKind.FromBody,
                    FQN.FromFormAttribute => ParameterBindingKind.FromForm,
                    FQN.FromHeaderAttribute => ParameterBindingKind.FromHeader,
                    FQN.FromQueryAttribute => ParameterBindingKind.FromQuery,
                    FQN.FromRouteAttribute => ParameterBindingKind.FromRoute,
                    _ => ParameterBindingKind.None
                };

                if (candidate == ParameterBindingKind.None) { continue; }

                bindingKind = candidate;
                bindingName = parameterAttribute.GetNamedArgument("Name").GetPrimitiveValue<string?>();

                break;
            }

            parameters.Add(new EndpointHandlerParameterModel {
                Name = parameter.Name,
                Type = type,
                IsCancellationToken = isCancellationToken,
                BindingKind = bindingKind,
                BindingName = bindingName
            });
        }

        return new EndpointHandlerModel { Method = handler, Parameters = [.. parameters] };
    }
}