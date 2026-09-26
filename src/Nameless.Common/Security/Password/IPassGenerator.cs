namespace Nameless.Security.Password;

/// <summary>
///     Defines methods to generate passwords.
/// </summary>
public interface IPassGenerator {
    /// <summary>
    ///     Generates a password.
    /// </summary>
    /// <param name="arguments">
    ///     Generator arguments.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous
    ///     operation. Where <see cref="Task{TResult}.Result" /> is a
    ///     <see cref="string" /> containing the generated password.
    /// </returns>
    Task<string> GenerateAsync(Arguments arguments, CancellationToken cancellationToken);
}