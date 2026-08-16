using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Generators.Diagnostics;
using Nameless.Generators.Infrastructure;
using Nameless.Generators.Models;
using Nameless.Generators.Web.Http.Endpoints.Diagnostics;

namespace Nameless.Generators.Web.Http.Endpoints.Infrastructure;

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
            Accessibility = accessibility ?? string.Empty,
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
        if (classSymbol.HasAllowAnonymousAttribute() && classSymbol.HasAuthorizeAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingAllowAnonymousVsAuthorizeAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableCookieRedirectAttribute() && classSymbol.HasAllowCookieRedirectAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableCookieRedirectVsAllowCookieRedirectAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableCorsAttribute() && classSymbol.HasEnableCorsAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableCorsVsEnableCorsAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableOutputCacheAttribute() && classSymbol.HasOutputCacheAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableOutputCacheVsOutputCacheAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableRateLimitingAttribute() && classSymbol.HasEnableRateLimitingAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableRateLimitingVsEnableRateLimitingAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableRequestTimeoutAttribute() && classSymbol.HasRequestTimeoutAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableRequestTimeoutVsRequestTimeoutAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (classSymbol.HasDisableValidationAttribute() && classSymbol.HasEnableValidationAttribute()) {
            diagnostics.Add(GeneratorDiagnostic.Create(
                descriptor: DiagnosticDescriptors.ConflictingDisableValidationVsEnableValidationAttributes,
                location: location,
                messageArgs: [classSymbol.Name]
            ));
        }

        return (Unit.Empty, [.. diagnostics]);
    }
}
