using System.Text;
using RootFromCore = Nameless.Root;

namespace Nameless.Security.Options {
    public sealed class AesCryptoOptions {
        #region Public Static Read-Only Properties

        public static AesCryptoOptions Default => new();

        #endregion

        #region Internal Properties

        internal Encoding Encoding
            => Encoding.GetEncoding(EncodingName);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the encryption/decryption key. It has a default value.
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Gets or sets the encoding. Default value is "utf-8".
        /// </summary>
        public string EncodingName { get; set; } = RootFromCore.Defaults.Encoding.BodyName;

        #endregion

        #region Public Constructors

        public AesCryptoOptions() {
            Key = Environment.GetEnvironmentVariable(Root.EnvTokens.AES_KEY)
                ?? Root.Defaults.AES_KEY;
        }

        #endregion
    }
}
