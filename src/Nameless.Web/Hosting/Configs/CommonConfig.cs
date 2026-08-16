using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Hosting.Configs;

/// <summary>
///     Common configurations
/// </summary>
public static class CommonConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers the common services to the application.
        ///     <para>
        ///         Common services are:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>Options</description>
        ///             </item>
        ///             <item>
        ///                 <description>ProblemDetails</description>
        ///             </item>
        ///             <item>
        ///                 <description>HttpContextAccessor</description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureCommon() {
            // Configures common services for the application.
            // These configurations should be moved to their own
            // files if they grow larger or more complex.

            // Adds the IOptions<> pattern for configuration binding.
            self.Services.AddOptions();

            // Adds the ProblemDetails service for creating problem details responses.
            self.Services.AddProblemDetails();

            // Adds the HttpContextAccessor service to access the current HTTP context.
            self.Services.AddHttpContextAccessor();

            return self;
        }
    }
}
