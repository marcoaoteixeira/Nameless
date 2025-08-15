using FluentValidation;

namespace Nameless.Web.Identity.Endpoints.v1.Auth.SignIn;

/// <summary>
///     Represents a validator for the <see cref="SignInInput"/> class.
/// </summary>
public class SignInInputValidator : AbstractValidator<SignInInput> {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="SignInInputValidator"/> class.
    /// </summary>
    public SignInInputValidator() {
        RuleFor(request => request.Email)
            .EmailAddress()
            .WithMessage($"Parameter '{nameof(SignInInput.Email)}' must be a valid email address.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage($"Parameter '{nameof(SignInInput.Password)}' cannot be empty or only whitespaces.");
    }
}