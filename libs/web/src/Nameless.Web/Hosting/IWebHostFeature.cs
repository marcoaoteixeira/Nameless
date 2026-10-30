using Microsoft.AspNetCore.Builder;
using Nameless.Hosting;

namespace Nameless.Web.Hosting;

/// <summary>
///     Represents a feature of the web host, like logging, authentication
///     or CORS, that <see cref="WebHostFactory"/> registers and uses when
///     building the web application.
/// </summary>
public interface IWebHostFeature {
    /// <summary>
    ///     Configures the middlewares or endpoints required by the feature.
    /// </summary>
    /// <param name="app">
    ///     The built <see cref="WebApplication"/>.
    /// </param>
    /// <param name="context">
    ///     The <see cref="HostFeatureContext"/> shared by every feature.
    /// </param>
    void Use(WebApplication app, HostFeatureContext context);
}