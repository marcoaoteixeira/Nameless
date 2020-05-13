namespace Nameless.Security {

    /// <summary>
    /// Defines methods to generate passwords.
    /// </summary>
	public interface IPasswordGenerator {
        #region	Methods

        /// <summary>
        /// Generates a password with the given parameters.
        /// </summary>
        /// <param name="minLength">The minimun length of the password.</param>
        /// <param name="maxLength">The maximun length of the password.</param>
        /// <returns>The <see cref="string"/> representation of the generated password.</returns>
        string Generate (int minLength, int maxLength);

        #endregion
    }
}