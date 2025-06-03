namespace Nameless.Security;

/// <summary>
///     Defines methods to generate passwords.
/// </summary>
public interface IPasswordGenerator {
    /// <summary>
    ///     Generates a password.
    /// </summary>
    /// <param name="parameters">Password generator parameters</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A <see cref="Task{TResult}" /> of <see cref="string" /> representing the password generation action.</returns>
    Task<string> GenerateAsync(PasswordParameters parameters, CancellationToken cancellationToken);
}