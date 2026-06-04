using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

public static class EndpointGroupExtractor {
    public static DiagnosticAwareResult<EndpointGroupModel> Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var location = new LocationModel(context.TargetNode.GetLocation());

        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.InvalidContextTargetSymbol,
                location: location,
                messageArgs: [context.TargetSymbol.Name]
            );
        }

        if (classSymbol.HasEndpointAttribute()) {
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

        var endpointGroupAttribute = classSymbol.GetEndpointGroupAttribute();
        if (endpointGroupAttribute is null) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ClassMissingEndpointGroupAttribute,
                location: location,
                messageArgs: [classSymbol.Name]
            );
        }

        var endpointGroupArgumentsModelExtraction = ExtractEndpointGroupArgumentsModel(
            endpointGroupAttribute,
            classSymbol,
            location
        );
        if (!endpointGroupArgumentsModelExtraction.Successful) {
            return endpointGroupArgumentsModelExtraction.Diagnostics;
        }
        
        var conventions = ConventionExtractor.Extract(classSymbol.GetAttributes(), cancellationToken);
        var conflicts = ClassHelper.ExtractAttributeConflicts(classSymbol, location, cancellationToken);
        var model = new EndpointGroupModel {
            Class = classModelExtraction.Model ?? throw new InvalidOperationException("Class model extraction failed."),
            Arguments = endpointGroupArgumentsModelExtraction.Model ?? throw new InvalidOperationException("Endpoint group arguments model extraction failed."),
            Conventions = conventions,
            Location = location,

            // These two properties will be provided later in the
            // extraction pipeline.
            Endpoints = [],
            ReportVersions = []
        };

        return (model, conflicts.Diagnostics);
    }

    private static DiagnosticAwareResult<EndpointGroupArgumentsModel> ExtractEndpointGroupArgumentsModel(AttributeData attribute, INamedTypeSymbol symbol, LocationModel location) {
        var diagnostics = new List<GeneratorDiagnostic>();

        var name = attribute.GetConstructorArgument(index: 0).GetPrimitiveValue<string?>();
        if (string.IsNullOrWhiteSpace(name)) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                DiagnosticDescriptors.EndpointGroupAttributeNameArgumentIsEmpty,
                location,
                messageArgs: [symbol.Name]
            ));
        }

        var prefix = attribute.GetConstructorArgument(index: 1).GetPrimitiveValue<string?>();
        if (string.IsNullOrWhiteSpace(prefix)) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                DiagnosticDescriptors.EndpointGroupAttributePrefixArgumentIsEmpty,
                location,
                messageArgs: [symbol.Name]
            ));
        }

        var model = new EndpointGroupArgumentsModel {
            Name = name ?? string.Empty,
            Prefix = prefix ?? string.Empty
        };

        return diagnostics.Count == 0 ? model : diagnostics.ToArray();
    }
}