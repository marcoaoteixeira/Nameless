using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.Validation;

namespace Nameless.Web.Filters.Validation;

public abstract class ValidationFilterBase {
    protected static async Task<Result<Nothing>> ValidateRequestObjectsAsync(IServiceProvider provider, IEnumerable<object?> arguments, CancellationToken cancellationToken) {
        if (!TryResolveValidation(provider, out var validation)) {
            return Nothing.Value;
        }

        var errors = new List<Error>();

        foreach (var argument in arguments) {
            if (argument is null) { continue; }

            var result = await validation.ValidateAsync(argument, cancellationToken);

            if (result.Failure) {
                errors.AddRange(result.Errors);
            }
        }

        return errors.ToArray();
    }

    protected static bool TryResolveValidation(IServiceProvider provider, [NotNullWhen(returnValue: true)] out IValidationService? output) {
        output = provider.GetService<IValidationService>();

        var hasService = output is not null;

        provider.GetLogger<ValidationFilterBase>()
                .OnCondition(!hasService)
                .ValidationServiceUnavailable();

        return hasService;
    }
}
