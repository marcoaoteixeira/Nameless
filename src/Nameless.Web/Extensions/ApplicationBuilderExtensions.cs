using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web;

/// <summary>
///     <see cref="IApplicationBuilder"/> extension methods.
/// </summary>
public static class ApplicationBuilderExtensions {
    private const string ENDPOINT_ROUTE_BUILDER_KEY = "__EndpointRouteBuilder";

    /// <param name="self">
    ///     The current instance of <see cref="IApplicationBuilder"/> class.
    /// </param>
    extension(IApplicationBuilder self) {
        /// <summary>
        ///     Gets the instance of <see cref="IEndpointRouteBuilder"/> that resides
        ///     in the <see cref="IApplicationBuilder.Properties"/>.
        /// </summary>
        public IEndpointRouteBuilder EndpointRouteBuilder {
            get {
                if (self.Properties.TryGetValue(ENDPOINT_ROUTE_BUILDER_KEY, out var output) &&
                    output is IEndpointRouteBuilder builder) {
                    return builder;
                }

                throw new InvalidOperationException($"'{nameof(IEndpointRouteBuilder)}' is unavailable.");
            }
        }
    }
}
