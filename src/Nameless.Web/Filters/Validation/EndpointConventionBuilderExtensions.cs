using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Filters.Validation;

/// <summary>
///     <see cref="RouteHandlerBuilder"/> extension methods.
/// </summary>
public static class EndpointConventionBuilderExtensions {
    /// <summary>
    ///     Adds the validation services to the endpoint.
    /// </summary>
    /// <param name="builder">
    ///     The current <see cref="RouteHandlerBuilder"/> instance.
    /// </param>
    /// <returns>
    ///     The current <see cref="RouteHandlerBuilder"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public static RouteHandlerBuilder WithRequestValidation(this RouteHandlerBuilder builder) {
        return builder.AddEndpointFilter<ValidateRequestEndpointFilter>();
    }
}
