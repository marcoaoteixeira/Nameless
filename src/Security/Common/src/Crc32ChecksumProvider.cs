using System;

namespace Nameless.Security {

    /// <summary>
    /// CRC-32 implementation of <see cref="IChecksumProvider"/>.
    /// </summary>
	public sealed class Crc32ChecksumProvider : IChecksumProvider {

        #region IChecksumProvider Members

        /// <inheritdoc />
        public string Generate (byte[] buffer) {
            Prevent.ParameterNull (buffer, nameof (buffer));

            return string.Format ("{0:X}", Crc32.Compute (buffer));
        }

        /// <inheritdoc />
		public bool Validate (string checksum, byte[] buffer) {
            Prevent.ParameterNullOrWhiteSpace (checksum, nameof (checksum));
            Prevent.ParameterNull (buffer, nameof (buffer));

            var actual = string.Format ("{0:X}", Crc32.Compute (buffer));

            return string.Equals (checksum, actual, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}