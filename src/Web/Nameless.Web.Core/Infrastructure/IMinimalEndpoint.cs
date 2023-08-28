using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web.Infrastructure {
    public interface IMinimalEndpoint {
        #region Properties

        public string Name { get; }
        public string Summary { get; }
        public string Description { get; }
        public string Group { get; }
        public int Version { get; }

        #endregion

        #region Methods

        IEndpointConventionBuilder Map(IEndpointRouteBuilder builder);

        #endregion
    }
}
