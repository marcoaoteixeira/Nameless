using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

namespace Nameless.Web;

internal sealed class NullApiVersionDescriptionProvider : IApiVersionDescriptionProvider {
    internal static readonly IApiVersionDescriptionProvider Instance = new NullApiVersionDescriptionProvider();

    static NullApiVersionDescriptionProvider() { }

    private NullApiVersionDescriptionProvider() { }

    IReadOnlyList<ApiVersionDescription> IApiVersionDescriptionProvider.ApiVersionDescriptions => [
        new(new ApiVersion(majorVersion: 1), groupName: string.Empty)
    ];
}