using System;
using System.Security.Cryptography;
using System.Text;

namespace Nameless.Security {

    /// <summary>
    /// A MD5 implementation of <see cref="IChecksumProvider"/>.
    /// </summary>
	public sealed class Md5ChecksumProvider : IChecksumProvider {

        #region IChecksumProvider Members

        /// <inheritdoc />
        public string Generate (byte[] buffer) {
            Prevent.ParameterNull (buffer, nameof (buffer));

            using (var md5 = MD5.Create ()) {
                return Encoding.UTF8.GetString (md5.ComputeHash (buffer));
            }
        }

        /// <inheritdoc />
		public bool Validate (string checksum, byte[] buffer) {
            Prevent.ParameterNullOrWhiteSpace (checksum, nameof (checksum));
            Prevent.ParameterNull (buffer, nameof (buffer));

            using (var md5 = MD5.Create ()) {
                var actual = Encoding.UTF8.GetString (md5.ComputeHash (buffer));

                return string.Equals (checksum, actual, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        #endregion
    }
}