using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public readonly record struct LocationModel {
    public string FilePath { get; }
    public int StartLine { get; }
    public int StartCharacter { get; }

    public LocationModel(Location location) {
        var startLinePosition = location.GetLineSpan().StartLinePosition;

        FilePath = location.SourceTree?.FilePath ?? string.Empty;
        StartLine = startLinePosition.Line;
        StartCharacter = startLinePosition.Character;
    }
}
