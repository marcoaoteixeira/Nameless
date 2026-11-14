using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Validation;
using Nameless.Validation.FluentValidation;

namespace Nameless.Hosting.Features.Validation;

/// <summary>
///     FluentValidation Host Feature
/// </summary>
public class FluentValidationHostFeature : HostFeature<FluentValidationFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAddTransient<IValidator, FluentValidationValidator>();
        builder.Services.TryAddEnumerable(options.Validators.Select(
            implementation => ServiceDescriptor.Transient(
                typeof(IFluentValidationValidator),
                implementation
            )
        ));
    }
}