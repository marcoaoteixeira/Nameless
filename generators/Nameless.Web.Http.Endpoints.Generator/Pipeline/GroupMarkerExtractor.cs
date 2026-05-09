using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class GroupMarkerExtractor {
    internal static GroupMarkerExtractionResult Extract(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol) {
            return GroupMarkerExtractionResult.Empty;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var location = context.TargetNode.GetLocation();
        var filePath = location.SourceTree?.FilePath ?? string.Empty;
        var lineSpan = location.GetLineSpan();
        var line = lineSpan.StartLinePosition.Line;
        var character = lineSpan.StartLinePosition.Character;

        var groupAttr = context.Attributes.FirstOrDefault(
            static attribute => attribute.AttributeClass?.ToDisplayString() == FQN.GROUP_ATTRIBUTE
        );

        if (groupAttr is null ||
            groupAttr.ConstructorArguments.Length < 2 ||
            groupAttr.ConstructorArguments[0].Value is not string name ||
            groupAttr.ConstructorArguments[1].Value is not string prefix) {
            return GroupMarkerExtractionResult.Empty;
        }

        if (string.IsNullOrWhiteSpace(name)) {
            return GroupMarkerExtractionResult.Failure([
                new GeneratorDiagnostic(
                    DiagnosticDescriptors.GroupMarkerEmptyName,
                    filePath, line, character,
                    [classSymbol.Name])
            ]);
        }

        var typeFqn = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var (versions, diagnostics) = ExtractDeclaredVersions(
            groupAttr,
            filePath,
            line,
            character,
            cancellationToken
        );

        return GroupMarkerExtractionResult.Success(
            new GroupMarkerModel(
                Name: name,
                Prefix: prefix,
                TypeFqn: typeFqn,
                DeclaredVersions: versions
            ),
            diagnostics
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

            if (VersionParser.TryParse(versionString, out var major, out var minor, out var patch)) {
                versions.Add(new VersionModel(major, minor, patch, Deprecated: false));

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

        return new VersionMetadata(
            [.. versions],
            [.. diagnostics]
        );
    }
}
