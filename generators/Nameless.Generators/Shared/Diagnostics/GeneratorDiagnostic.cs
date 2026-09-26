using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Nameless.Generators.Shared.Models;

namespace Nameless.Generators.Shared.Diagnostics;

// Stores location-safe diagnostic info (Location is not equatable for incremental cache).
public record GeneratorDiagnostic {
    public DiagnosticDescriptor Descriptor { get; }
    public string FilePath { get; }
    public int StartLine { get; }
    public int StartCharacter { get; }
    public string[] MessageArgs { get; }

    private GeneratorDiagnostic(DiagnosticDescriptor descriptor, string filePath, int startLine, int startCharacter, string[] messageArgs) {
        Descriptor = descriptor;
        FilePath = filePath;
        StartLine = startLine;
        StartCharacter = startCharacter;
        MessageArgs = messageArgs;
    }

    public static GeneratorDiagnostic Create(DiagnosticDescriptor descriptor, LocationModel location, params string[] messageArgs) {
        return new GeneratorDiagnostic(
            descriptor: descriptor,
            filePath: location.FilePath,
            startLine: location.StartLine,
            startCharacter: location.StartCharacter,
            messageArgs: messageArgs
        );
    }

    public Diagnostic ToDiagnostic() {
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
