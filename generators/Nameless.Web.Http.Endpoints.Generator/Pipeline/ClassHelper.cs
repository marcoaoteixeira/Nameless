using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

public static class ClassHelper {
    public static DiagnosticAwareResult<ClassModel> ExtractClassModel(ClassDeclarationSyntax syntax, INamedTypeSymbol classSymbol, LocationModel location, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        
        var diagnostics = new List<GeneratorDiagnostic>();

        var isPartial = syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        if (!isPartial) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassTypeModifierMustBePartial);
        }

        var isAbstract = syntax.Modifiers.Any(SyntaxKind.AbstractKeyword);
        if (isAbstract) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassTypeModifierMustNotBeAbstract);
        }

        var accessibility = classSymbol.DeclaredAccessibility switch {
            Accessibility.Public => nameof(Accessibility.Public).ToLowerInvariant(),
            Accessibility.Internal => nameof(Accessibility.Internal).ToLowerInvariant(),
            _ => null
        };

        if (string.IsNullOrWhiteSpace(accessibility)) {
            IncludeDiagnostic(DiagnosticDescriptors.ClassAccessorModifierMustBePublicOrInternal);
        }

        var model = new ClassModel {
            Namespace = classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToDisplayString(),
            Name = classSymbol.Name,
            AccessorModifier = accessibility ?? string.Empty,
        };

        return (model, [.. diagnostics]);

        void IncludeDiagnostic(DiagnosticDescriptor descriptor) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor,
                location,
                messageArgs: [classSymbol.Name]
            ));
        }
    }

    public static DiagnosticAwareResult<Unit> ExtractAttributeConflicts(INamedTypeSymbol classSymbol, LocationModel location, CancellationToken cancellationToken) {
        var diagnostics = new List<GeneratorDiagnostic>();

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasAllowAnonymousAttribute() && classSymbol.HasUseAuthorizationAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingAllowAnonymousAndUseAuthorizationAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableCookieRedirectAttribute() && classSymbol.HasAllowCookieRedirectAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingAllowAndDisableCookieRedirectAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableCorsAttribute() && classSymbol.HasUseCorsAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableAndUseCorsAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableOutputCacheAttribute() && classSymbol.HasUseOutputCacheAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableAndUseOutputCacheAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableRateLimitingAttribute() && classSymbol.HasUseRateLimitingAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableAndUseRateLimitingAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableRequestTimeoutAttribute() && classSymbol.HasUseRequestTimeoutAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableAndUseRequestTimeoutAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        return (Unit.Empty, [.. diagnostics]);
    }
}
