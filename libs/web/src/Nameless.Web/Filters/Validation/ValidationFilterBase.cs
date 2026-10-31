using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Nameless.ObjectModel;
using Nameless.Validation;

namespace Nameless.Web.Filters.Validation;

/// <summary>
///     Base class for filters that need to validate request objects.
/// </summary>
public abstract class ValidationFilterBase {
    /// <summary>
    ///     Validates the given request objects, if a validation service is available.
    /// </summary>
    /// <param name="provider">
    ///     The current <see cref="IServiceProvider"/> instance.
    /// </param>
    /// <param name="arguments">
    ///     The request objects to validate.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous
    ///     execution. The result of the task will be the validation result.
    /// </returns>
    protected async Task<ValidationResult> ValidateRequestObjectsAsync(IServiceProvider provider, IEnumerable<object?> arguments, CancellationToken cancellationToken) {
        if (!TryResolveValidation(provider, out var validation)) {
            return ValidationResult.Successful;
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

    /// <summary>
    ///     Tries to resolve a validation service.
    /// </summary>
    /// <param name="provider">
    ///     The current <see cref="IServiceProvider"/> instance.
    /// </param>
    /// <param name="output">
    ///     The resolved validation service, if available.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the validation service is available;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    protected bool TryResolveValidation(IServiceProvider provider, [NotNullWhen(returnValue: true)] out IValidator? output) {
        output = provider.GetService<IValidator>();

        var isServiceUnavailable = output is not null;
        if (!isServiceUnavailable) { return isServiceUnavailable; }

        var logger = provider.GetLogger<ValidationFilterBase>();

        CommonLog.Warning(
            logger,
            message: $"Service '{nameof(IValidator)}' is unavailable.",
            tag: GetType().Tag
        );

        return isServiceUnavailable;
    }
}
