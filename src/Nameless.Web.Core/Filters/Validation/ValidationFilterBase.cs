using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.Validation;

namespace Nameless.Web.Filters.Validation;

public abstract class ValidationFilterBase {
    protected static async Task<Result<bool>> ValidateRequestObjectsAsync(IServiceProvider provider, IEnumerable<object?> arguments, CancellationToken cancellationToken) {
        if (!TryResolveValidationService(provider, out var validation)) {
            return true;
        }

        var errors = new List<Error>();

        foreach (var argument in arguments.Cast<object>()) {
            if (!ValidateAttribute.IsPresent(argument)) { continue; }

            var result = await validation.ValidateAsync(
                argument,
                cancellationToken
            );

            errors.AddRange(result.ToErrors());
        }

        return errors.ToArray();
    }

    protected static bool TryResolveValidationService(IServiceProvider provider, [NotNullWhen(returnValue: true)] out IValidationService? output) {
        output = provider.GetService<IValidationService>();

        var hasService = output is not null;

        provider.GetLogger<ValidationFilterBase>()
                .OnCondition(!hasService)
                .ValidationServiceUnavailable();

        return hasService;
    }
}
