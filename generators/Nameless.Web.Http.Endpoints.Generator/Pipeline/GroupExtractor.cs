using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Models;
using Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class GroupExtractor {
    internal static GroupExtractionResult Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return GroupExtractionResult.Empty;
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
            return GroupExtractionResult.Failure([
                new GeneratorDiagnostic(
                    Descriptor: DiagnosticDescriptors.ClassMustBePartial,
                    FilePath: filePath,
                    StartLine: line,
                    StartCharacter: character,
                    MessageArgs: [classSymbol.Name]
                )
            ]);
        }

        var groupAttr = context.Attributes.FirstOrDefault(
            static attribute => attribute.AttributeClass?.ToDisplayString() == FQN.ENDPOINT_GROUPING_ATTRIBUTE
        );

        if (groupAttr is null ||
            groupAttr.ConstructorArguments.Length < 2 ||
            groupAttr.ConstructorArguments[0].Value is not string name ||
            groupAttr.ConstructorArguments[1].Value is not string prefix) {
            return GroupExtractionResult.Empty;
        }

        if (string.IsNullOrWhiteSpace(name)) {
            return GroupExtractionResult.Failure([
                new GeneratorDiagnostic(
                    Descriptor: DiagnosticDescriptors.GroupMarkerEmptyName,
                    FilePath: filePath,
                    StartLine: line,
                    StartCharacter: character,
                    MessageArgs: [classSymbol.Name]
                )
            ]);
        }

        var typeFqn = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var className = classSymbol.Name;
        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();
        var accessModifier = classSymbol.DeclaredAccessibility switch {
            Accessibility.Public => "public",
            _ => "internal"
        };

        var (versions, versionDiagnostics) = ExtractDeclaredVersions(
            groupAttr, filePath, line, character, cancellationToken
        );

        var metadata = ExtractGroupMetadata(classSymbol);

        var allDiagnostics = versionDiagnostics;

        if (metadata is { RequireAuthorization: true, AllowAnonymous: true }) {
            allDiagnostics = [
                ..versionDiagnostics,
                new GeneratorDiagnostic(
                    Descriptor: DiagnosticDescriptors.ConflictingAuthAttributes,
                    FilePath: filePath,
                    StartLine: line,
                    StartCharacter: character,
                    MessageArgs: [classSymbol.Name]
                )
            ];
        }

        return GroupExtractionResult.Success(
            new GroupMarkerModel(
                Name: name,
                Prefix: prefix,
                TypeFqn: typeFqn,
                ClassName: className,
                Namespace: namespaceName,
                AccessModifier: accessModifier,
                DeclaredVersions: versions,
                RateLimitingPolicy: metadata.RateLimitingPolicy,
                DisableRateLimiting: metadata.DisableRateLimiting,
                DisableAntiforgery: metadata.DisableAntiforgery,
                DisableHttpMetrics: metadata.DisableHttpMetrics,
                OutputCachePolicy: metadata.OutputCachePolicy,
                CorsPolicy: metadata.CorsPolicy,
                AllowAnonymous: metadata.AllowAnonymous,
                RequireAuthorization: metadata.RequireAuthorization,
                AuthorizationPolicy: metadata.AuthorizationPolicy,
                RequestTimeoutPolicy: metadata.RequestTimeoutPolicy,
                DisableRequestTimeout: metadata.DisableRequestTimeout,
                AllowCookieRedirect: metadata.AllowCookieRedirect,
                FilterTypeNames: metadata.FilterTypeNames
            ),
            allDiagnostics
        );
    }

    private static VersionMetadata ExtractDeclaredVersions(AttributeData groupAttr, string filePath, int line, int character, CancellationToken cancellationToken) {
        var versionsArg = groupAttr.NamedArguments.FirstOrDefault(
            static constant => constant.Key == "Versions"
        ).Value;

        if (versionsArg.Kind != TypedConstantKind.Array) {
            return new VersionMetadata([], []);
        }

        var versions = ImmutableArray.CreateBuilder<VersionModel>(versionsArg.Values.Length);
        var diagnostics = ImmutableArray.CreateBuilder<GeneratorDiagnostic>();

        foreach (var constant in versionsArg.Values) {
            cancellationToken.ThrowIfCancellationRequested();

            if (constant.Value is not string versionString) {
                continue;
            }

            if (VersionParser.TryParse(versionString, out var major, out var minor)) {
                versions.Add(new VersionModel(major, minor, Patch: 0, Deprecated: false));

                continue;
            }

            diagnostics.Add(new GeneratorDiagnostic(
                Descriptor: DiagnosticDescriptors.InvalidVersionString,
                FilePath: filePath,
                StartLine: line,
                StartCharacter: character,
                MessageArgs: [versionString, "Versions"]
            ));
        }

        return new VersionMetadata([.. versions], [.. diagnostics]);
    }

    private static GroupMetadata ExtractGroupMetadata(INamedTypeSymbol classSymbol) {
        var rateLimitAttr = classSymbol.GetEnableRateLimitingAttribute();
        var disableRateLimitAttr = classSymbol.GetDisableRateLimitingAttribute();
        var disableMetricsAttr = classSymbol.GetDisableHttpMetricsAttribute();
        var outputCacheAttr = classSymbol.GetOutputCacheAttribute();
        var corsAttr = classSymbol.GetEnableCorsAttribute();
        var allowAnonAttr = classSymbol.GetAllowsAnonymousAttribute();
        var authorizeAttr = classSymbol.GetAuthorizeAttribute();
        var requestTimeoutAttr = classSymbol.GetRequestTimeoutAttribute();
        var disableTimeoutAttr = classSymbol.GetDisableRequestTimeoutAttribute();

        var filterTypeNames = classSymbol.GetFilterTypeNames();

        return new GroupMetadata(
            RateLimitingPolicy: rateLimitAttr.GetCtorArgValue<string?>(0),
            DisableRateLimiting: disableRateLimitAttr is not null,
            DisableAntiforgery: classSymbol.DisableAntiforgery(),
            DisableHttpMetrics: disableMetricsAttr is not null,
            OutputCachePolicy: outputCacheAttr.GetNamedArgumentValue<string?>("PolicyName"),
            CorsPolicy: corsAttr.GetCtorArgValue<string?>(0),
            AllowAnonymous: allowAnonAttr is not null,
            RequireAuthorization: authorizeAttr is not null,
            AuthorizationPolicy: authorizeAttr.GetCtorArgValue<string?>(0),
            RequestTimeoutPolicy: requestTimeoutAttr.GetCtorArgValue<string?>(0),
            DisableRequestTimeout: disableTimeoutAttr is not null,
            AllowCookieRedirect: classSymbol.AllowCookieRedirect(),
            FilterTypeNames: filterTypeNames
        );
    }

    private readonly record struct GroupMetadata(
        string? RateLimitingPolicy,
        bool DisableRateLimiting,
        bool DisableAntiforgery,
        bool DisableHttpMetrics,
        string? OutputCachePolicy,
        string? CorsPolicy,
        bool AllowAnonymous,
        bool RequireAuthorization,
        string? AuthorizationPolicy,
        string? RequestTimeoutPolicy,
        bool DisableRequestTimeout,
        bool AllowCookieRedirect,
        ImmutableArray<string> FilterTypeNames
    );
}
