using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Hosting.Options;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers the error handling services and uses the exception handler middleware.
/// </summary>
public class ExceptionHandlingWebHostFeature : HostFeature<ErrorHandlingFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        
        builder.Services.TryAdd(options.ExceptionHandlers.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IExceptionHandler),
                implementation
            )
        ));
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        app.UseExceptionHandler(
            ActionHelper.FromDelegate(options.ConfigureExceptionHandlerOptions)
        );
    }
}
