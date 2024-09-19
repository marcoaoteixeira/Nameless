namespace Nameless.Security;

/// <summary>
/// Defines methods to generate passwords.
/// </summary>
public interface IPasswordGenerator {
    /// <summary>
    /// Generates a password.
    /// </summary>
    /// <param name="options">Password generator options</param>
    /// <returns>The <see cref="string"/> representation of the generated password.</returns>
    string Generate(PasswordOptions options);
}