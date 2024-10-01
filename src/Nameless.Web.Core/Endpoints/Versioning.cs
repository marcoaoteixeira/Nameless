namespace Nameless.Web.Endpoints;

public sealed record Versioning {
    public int Version { get; init; }
    public bool IsDeprecated { get; init; }
    public int MapToVersion { get; init; }

    public static Versioning For(int version, bool isDeprecated = false, int mapToVersion = 0)
        => new() {
            Version = Prevent.Argument.LowerThan(version, minValue: 0),
            IsDeprecated = isDeprecated,
            MapToVersion = Prevent.Argument.LowerThan(mapToVersion, minValue: 0)
        };
}