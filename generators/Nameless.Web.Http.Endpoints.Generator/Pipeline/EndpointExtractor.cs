using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

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
                descriptor: DiagnosticDescriptors.EndpointHasMisplacedEndpointGroupAttribute,
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
                descriptor: DiagnosticDescriptors.EndpointMissingEndpointAttribute,
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
        var conventions = ConventionExtractor.Extract(classSymbol.GetAttributes(), cancellationToken);
        var model = new EndpointModel {
            Class = classModelExtraction.Model ?? throw new InvalidOperationException("Class model extraction failed."),
            Arguments = endpointArgumentsModelExtraction.Model ?? throw new InvalidOperationException("Endpoint arguments model extraction failed."),
            Conventions = conventions,
            Handler = endpointHandlerModelExtraction.Model ?? throw new InvalidOperationException("Endpoint handler model extraction failed."),
            Location = location
        };

        return (model, conflicts.Diagnostics);
    }
    
    private static DiagnosticAwareResult<EndpointArgumentsModel> ExtractEndpointArgumentsModel(AttributeData attributeData, INamedTypeSymbol classSymbol, LocationModel location) {
        if (attributeData.AttributeClass is null) {
            return new EndpointArgumentsModel();
        }

        var httpVerb = attributeData.AttributeClass.GetTypeArgument(index: 0)?.GetFullyQualifiedName() switch {
            FQN.Nameless.HttpVerbPost => HttpVerbs.Post,
            FQN.Nameless.HttpVerbPut => HttpVerbs.Put,
            FQN.Nameless.HttpVerbPatch => HttpVerbs.Patch,
            FQN.Nameless.HttpVerbDelete => HttpVerbs.Delete,
            FQN.Nameless.HttpVerbHead => HttpVerbs.Head,
            FQN.Nameless.HttpVerbOptions => HttpVerbs.Options,
            _ => HttpVerbs.Get
        };

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
            descriptor: DiagnosticDescriptors.EndpointAttributeInvalidVersion,
            location: location,
            messageArgs: [classSymbol.Name, version]
        );
    }

    private static DiagnosticAwareResult<EndpointHandlerModel> ExtractEndpointHandlerModel(INamedTypeSymbol classSymbol, LocationModel location, CancellationToken cancellationToken) {
        var methodSymbol = classSymbol.GetMembers(EndpointClass.HandlerMethodName)
                                      .SingleOrDefault(member => member is IMethodSymbol);

        if (methodSymbol is not IMethodSymbol handler) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.EndpointMissingHandlerMethod,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        if (handler.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.EndpointHandlerMethodMustBePublic,
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
            var isCancellationToken = type == FQN.System.CancellationToken;

            var bindingKind = ParameterBindingKind.None;
            string? bindingName = null;

            foreach (var parameterAttribute in parameter.GetAttributes()) {
                var candidate = parameterAttribute.AttributeClass?.ToDisplayString() switch {
                    FQN.Microsoft.AsParametersAttribute => ParameterBindingKind.AsParameters,
                    FQN.Microsoft.FromBodyAttribute => ParameterBindingKind.FromBody,
                    FQN.Microsoft.FromFormAttribute => ParameterBindingKind.FromForm,
                    FQN.Microsoft.FromHeaderAttribute => ParameterBindingKind.FromHeader,
                    FQN.Microsoft.FromQueryAttribute => ParameterBindingKind.FromQuery,
                    FQN.Microsoft.FromRouteAttribute => ParameterBindingKind.FromRoute,
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