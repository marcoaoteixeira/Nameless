using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web.Infrastructure {
    public interface IMinimalEndpoint {
        #region Properties

        string Name { get; }
        string Summary { get; }
        string Description { get; }
        string Group { get; }
        int Version { get; }

        #endregion

        #region Methods

        IEndpointConventionBuilder Map(IEndpointRouteBuilder builder);

        #endregion
    }
}
