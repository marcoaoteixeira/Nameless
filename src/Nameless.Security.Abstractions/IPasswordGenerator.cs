namespace Nameless.Security;

/// <summary>
///     Defines methods to generate passwords.
/// </summary>
public interface IPasswordGenerator {
    /// <summary>
    ///     Generates a password.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    ///     Returns a <see cref="Task{TResult}" /> that represents the
    ///     asynchronous operation.
    ///     Where <see cref="Task{TResult}.Result" /> is a
    ///     <see cref="string" /> containing the generated password.
    /// </returns>
    Task<string> GenerateAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Generates a password.
    /// </summary>
    /// <param name="parameters">Password generator parameters</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    ///     Returns a <see cref="Task{TResult}" /> that represents the
    ///     asynchronous operation.
    ///     Where <see cref="Task{TResult}.Result" /> is a
    ///     <see cref="string" /> containing the generated password.
    /// </returns>
    Task<string> GenerateAsync(PasswordParameters parameters, CancellationToken cancellationToken);
}