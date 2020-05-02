namespace Nameless.Security {

    /// <summary>
    /// Defines methods to calculate checksum.
    /// </summary>
	public interface IChecksumProvider {

        #region Methods

        /// <summary>
        /// Generates a checksum for a buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="string"/> representation of the checksum.</returns>
        string Generate (byte[] buffer);

        /// <summary>
        /// Validates the checksum of a buffer.
        /// </summary>
        /// <param name="checksum">The checksum to validate.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
		bool Validate (string checksum, byte[] buffer);

        #endregion Methods
    }
}