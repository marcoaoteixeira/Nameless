namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record VersionModel(
    int Major,
    int Minor,
    int Patch,
    bool Deprecated
);
