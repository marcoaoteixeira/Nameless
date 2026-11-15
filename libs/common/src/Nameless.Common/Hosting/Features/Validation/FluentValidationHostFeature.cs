using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Validation;

namespace Nameless.Hosting.Features.Validation;

/// <summary>
///     FluentValidation Host Feature
/// </summary>
public class FluentValidationHostFeature : HostFeature<FluentValidationFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAddTransient<IValidationService, ValidationService>();
        builder.Services.TryAddEnumerable(options.Validators.Select(
            implementation => ServiceDescriptor.Transient(
                typeof(IValidator),
                implementation
            )
        ));
    }
}