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
