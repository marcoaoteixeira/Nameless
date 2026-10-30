using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     FluentValidation Host Feature
/// </summary>
public class FluentValidationHostFeature : HostFeature<FluentValidationFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);

        builder.Services.TryAddTransient<IValidator, FluentValidationValidator>();
        builder.Services.TryAddEnumerable(options.Validators.Select(
            implementation => ServiceDescriptor.Transient(
                typeof(IFluentValidationValidator),
                implementation
            )
        ));
    }
}