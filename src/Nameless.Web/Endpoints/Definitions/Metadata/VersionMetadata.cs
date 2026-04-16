using Nameless.Web.OpenApi;

namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents metadata about the version of an endpoint.
/// </summary>
/// <param name="Number">
///     The number of the version.
/// </param>
/// <param name="Stability">
///     The stability of the endpoint.
/// </param>
public readonly record struct VersionMetadata(int Number, Stability Stability);