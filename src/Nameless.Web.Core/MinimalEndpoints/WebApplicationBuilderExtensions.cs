using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Web.MinimalEndpoints.Infrastructure;

namespace Nameless.Web.MinimalEndpoints;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures minimal endpoint services for the application,
        ///     including OpenAPI, versioning, and all implemented endpoints.
        /// </summary>
        /// <remarks>
        ///     This method sets up essential services required for minimal
        ///     endpoint functionality, including API versioning, and core
        ///     application services.
        ///     Use the <paramref name="registration"/> parameter to customize
        ///     endpoint options, such as enabling or disabling specific
        ///     features.
        /// </remarks>
        /// <param name="registration">
        ///     An optional delegate to registration
        ///     <see cref="MinimalEndpointsRegistrationSettings"/> for
        ///     customizing endpoint behavior. If not provided, default options
        ///     are used.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> so other
        ///     actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterMinimalEndpoints(Action<MinimalEndpointsRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.Services
                .AddApiVersioning(settings.ConfigureApiVersioning)
                .AddApiExplorer(settings.ConfigureApiExplorer);

            self.Services.TryAddSingleton(new EndpointTypeCollection(settings.Endpoints));
            self.Services.TryAddSingleton<IServiceFactory, ServiceFactory>();
            self.Services.TryAddSingleton<IEndpointFactory, EndpointFactory>();

            return self;
        }
    }
}