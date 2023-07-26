using Microsoft.AspNetCore.Routing;

namespace Nameless.Microservice.Infrastructure {
    public interface IEndpoint {
        #region Methods

        void Map(IEndpointRouteBuilder builder);

        #endregion
    }
}
