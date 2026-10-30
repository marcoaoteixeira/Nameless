using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Hosting.Features;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers the services every web host needs: options, problem
///     details, the HTTP context accessor and <see cref="TimeProvider"/>.
/// </summary>
public class CommonWebHostFeature : CommonHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        base.Register(builder, context);

        builder.Services.AddProblemDetails();
        builder.Services.AddHttpContextAccessor();
    }
}
