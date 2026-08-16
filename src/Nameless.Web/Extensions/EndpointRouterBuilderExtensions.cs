using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Web;

/// <summary>
///     <see cref="IEndpointRouteBuilder"/> extension methods
/// </summary>
public static class EndpointRouteBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="IEndpointRouteBuilder"/> instance.
    /// </param>
    extension(IEndpointRouteBuilder self) {
        /// <summary>
        ///     Maps the HTTP QUERY method.
        /// </summary>
        /// <param name="pattern">
        ///     The route pattern.
        /// </param>
        /// <param name="requestDelegate">
        ///     The request delegate.
        /// </param>
        /// <returns>
        ///     The <see cref="IEndpointConventionBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IEndpointConventionBuilder MapQuery([StringSyntax(Syntaxes.Route)] string pattern, RequestDelegate requestDelegate) {
            return self.MapMethods(pattern, [HttpMethods.Query], requestDelegate);
        }

        /// <summary>
        ///     Maps the HTTP QUERY method.
        /// </summary>
        /// <param name="pattern">
        ///     The route pattern.
        /// </param>
        /// <param name="delegate">
        ///     The request delegate.
        /// </param>
        /// <returns>
        ///     The <see cref="RouteHandlerBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public RouteHandlerBuilder MapQuery([StringSyntax(Syntaxes.Route)] string pattern, Delegate @delegate) {
            return self.MapMethods(pattern, [HttpMethods.Query], @delegate);
        }
    }
}
