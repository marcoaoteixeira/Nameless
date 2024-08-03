using Nameless.Security.Crypto;

namespace Nameless.Security.Options {
    public sealed record RijndaelCryptoOptions {
        #region Public Constants

        public const int MAXIMUM_ALLOWED_SALT_SIZE = 255;
        public const int MINIMUM_ALLOWED_SALT_SIZE = 4;
        public const int MINIMUM_PASSWORD_ITERATIONS = 1;

        #endregion

        #region Public Static Read-Only Properties

        public static RijndaelCryptoOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the pass phrase.
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. Passphrase value must be kept in
        /// secret.
        /// </summary>
        public string Passphrase { get; set; } = Root.Defaults.RIJNDAEL_PASS_PHRASE;

        /// <summary>
        /// Gets or sets the initialization vector.
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </summary>
        private string? _iv;
        public string IV {
            get => _iv ??= Root.Defaults.RIJNDAEL_IV;
            set => _iv = value;
        }
        /// <summary>
        /// Gets or sets the salt.
        /// Salt value used for password hashing during key generation. This is
        /// not the same as the salt we will use during encryption. This parameter
        /// can be any string.
        /// </summary>
        private string? _salt;
        public string Salt {
            get => _salt ??= Root.Defaults.RIJNDAEL_SALT;
            set => _salt = value;
        }
        /// <summary>
        /// Gets or sets the key size.
        /// If key size is not specified, longest 256-bit key will be used.
        /// </summary>
        public KeySize KeySize { get; set; }
        /// <summary>
        /// Gets or sets the minimum salt size.
        /// This value should not be smaller than 4 bytes,
        /// because we use the first 4 bytes of salt to store its length.
        /// </summary>
        private int _minimumSaltSize = 16;
        public int MinimumSaltSize {
            get => _minimumSaltSize;
            set => _minimumSaltSize = value is >= MINIMUM_ALLOWED_SALT_SIZE and <= MAXIMUM_ALLOWED_SALT_SIZE
                ? value
                : MINIMUM_ALLOWED_SALT_SIZE;
        }
        /// <summary>
        /// Gets or sets the maximum salt size.
        /// This value should not be longer than 255 bytes,
        /// because we have only 1 byte to store its length.
        /// </summary>
        private int _maximumSaltSize = 128;
        public int MaximumSaltSize {
            get => _maximumSaltSize;
            set => _maximumSaltSize = value is >= MINIMUM_ALLOWED_SALT_SIZE and <= MAXIMUM_ALLOWED_SALT_SIZE
                ? value
                : MAXIMUM_ALLOWED_SALT_SIZE;
        }
        /// <summary>
        /// Gets or sets the password iterations.
        /// Number of iterations used to hash password. More iterations are
        /// considered more secure but may take longer. Minimum of 1.
        /// </summary>
        private int _passwordIterations = 1;
        public int PasswordIterations {
            get => _passwordIterations;
            set => _passwordIterations = value >= MINIMUM_PASSWORD_ITERATIONS
                ? value
                : MINIMUM_PASSWORD_ITERATIONS;
        }

        #endregion
    }
}
