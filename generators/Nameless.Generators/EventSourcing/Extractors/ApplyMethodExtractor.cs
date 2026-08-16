using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Generators.Diagnostics;
using Nameless.Generators.EventSourcing.Diagnostics;
using Nameless.Generators.EventSourcing.Model;
using Nameless.Generators.Infrastructure;
using Nameless.Generators.Models;

namespace Nameless.Generators.EventSourcing.Extractors;

/// <summary>
///     Extracts an <see cref="ApplyMethodModel"/> from a method decorated
///     with <c>[Apply]</c>.
/// </summary>
public static class ApplyMethodExtractor {
    /// <summary>
    ///     Extracts an <see cref="ApplyMethodModel"/> from <paramref name="context"/>.
    ///     Returns diagnostics instead of a model when validation fails.
    /// </summary>
    public static DiagnosticAwareResult<ApplyMethodModel> Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var location = new LocationModel(context.TargetNode.GetLocation());

        if (context.TargetSymbol is not IMethodSymbol { ContainingType: { } classSymbol } methodSymbol ||
            context.TargetNode is not MethodDeclarationSyntax { Parent: ClassDeclarationSyntax classDeclarationSyntax }) {
            return GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.WrongTargetSymbol,
                location: location,
                messageArgs: [context.TargetSymbol.Name]
            );
        }

        var diagnostics = new List<GeneratorDiagnostic>();

        if (!classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassTypeModifierMustBePartial, classSymbol.Name);
        }

        if (classDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassTypeModifierMustNotBeAbstract, classSymbol.Name);
        }

        var aggregateRootSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
            EventSourcingConstants.AggregateRootClass.FullNameWithArity
        );
        if (!classSymbol.DerivesFrom(aggregateRootSymbol)) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassMustDeriveFromAggregateRoot, classSymbol.Name);
        }

        if (methodSymbol.IsStatic) {
            IncludeDiagnostic(DiagnosticDescriptors.ApplyMethodMustNotBeStatic, methodSymbol.Name);
        }

        string? eventTypeFullName = null;
        if (methodSymbol.Parameters.Length == 1) {
            var parameterType = methodSymbol.Parameters[0].Type;

            if (parameterType.TypeKind == TypeKind.Interface || parameterType.IsAbstract) {
                IncludeDiagnostic(DiagnosticDescriptors.ApplyMethodParameterMustBeConcrete, methodSymbol.Name);
            }

            var eventInterfaceSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(EventSourcingConstants.FQN.EventInterface);
            var implementsEvent = eventInterfaceSymbol is not null &&
                                  parameterType.AllInterfaces.Contains(eventInterfaceSymbol, SymbolEqualityComparer.Default);

            if (!implementsEvent) {
                IncludeDiagnostic(DiagnosticDescriptors.ApplyMethodParameterMustImplementIEvent, methodSymbol.Name);
            }

            eventTypeFullName = parameterType.GetFullyQualifiedName();
        }
        else { IncludeDiagnostic(DiagnosticDescriptors.ApplyMethodMustHaveExactlyOneParameter, methodSymbol.Name); }

        if (diagnostics.Count > 0) { return diagnostics.ToArray(); }

        var classModel = new ClassModel { 
            Namespace = classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToDisplayString(),
            Name = classSymbol.Name,
            Accessibility = string.Empty
        };

        return new ApplyMethodModel(classModel, eventTypeFullName!, methodSymbol.Name, location);

        void IncludeDiagnostic(DiagnosticDescriptor descriptor, string name) {
            diagnostics.Add(GeneratorDiagnostic.Create(descriptor, location, messageArgs: [name]));
        }
    }
}
