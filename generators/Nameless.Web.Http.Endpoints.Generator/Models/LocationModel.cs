using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record LocationModel {
    public string FilePath { get; init; }
    public int StartLine { get; init; }
    public int StartCharacter { get; init; }

    public LocationModel(Location location) {
        var startLinePosition = location.GetLineSpan().StartLinePosition;

        FilePath = location.SourceTree?.FilePath ?? string.Empty;
        StartLine = startLinePosition.Line;
        StartCharacter = startLinePosition.Character;
    }
}
