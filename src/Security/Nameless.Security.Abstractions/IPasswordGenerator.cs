namespace Nameless.Security {

    /// <summary>
    /// Defines methods to generate passwords.
    /// </summary>
    public interface IPasswordGenerator {

        #region	Methods

        /// <summary>
        /// Generates a password.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the generated password.</returns>
        string Generate();

        #endregion
    }
}
