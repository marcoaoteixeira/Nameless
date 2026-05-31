using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Extensions;

public static class EndpointModelExtensions {
    extension(IEnumerable<EndpointModel> self) {
        public VersionModel[] CollectVersions() {
            return self.Select(static endpoint => endpoint.Arguments.Version != default ? endpoint.Arguments.Version : VersionModel.V1)
                       .Distinct()
                       .ToArray();
        }
    }
}
