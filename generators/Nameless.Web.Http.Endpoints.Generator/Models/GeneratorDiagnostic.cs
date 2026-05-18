using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

// Stores location-safe diagnostic info (Location is not equatable for incremental cache).
internal sealed record GeneratorDiagnostic(
    DiagnosticDescriptor Descriptor,
    string FilePath,
    int StartLine,
    int StartCharacter,
    ImmutableArray<string> MessageArgs
) {
    internal static GeneratorDiagnostic Create(DiagnosticDescriptor descriptor, Location location, params string[] messageArgs) {
        var line = location.GetLineSpan();

        return new GeneratorDiagnostic(
            Descriptor: descriptor,
            FilePath: location.SourceTree?.FilePath ?? string.Empty,
            StartLine: line.StartLinePosition.Line,
            StartCharacter: line.StartLinePosition.Character,
            MessageArgs: [.. messageArgs]
        );
    }

    internal Diagnostic ToDiagnostic() {
        var location = Location.Create(
            filePath: FilePath,
            textSpan: default,
            lineSpan: new LinePositionSpan(
                new LinePosition(StartLine, StartCharacter),
                new LinePosition(StartLine, StartCharacter)
            )
        );

        return Diagnostic.Create(Descriptor, location, [.. MessageArgs]);
    }
}
