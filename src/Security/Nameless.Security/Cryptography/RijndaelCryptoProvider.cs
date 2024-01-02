﻿using System.Security.Cryptography;
using System.Text;

namespace Nameless.Security.Cryptography {
    /// <summary>
    /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and
    /// decrypt data. As long as it is initialized with the same constructor
    /// parameters, the class will use the same key. Before performing encryption,
    /// the class can prepend random bytes to plain text and generate different
    /// encrypted values from the same plain text, encryption key, initialization
    /// vector, and other parameters. This class is thread-safe.
    /// </summary>
    /// <remarks>
    /// Be careful when performing encryption and decryption. There is a bug
    /// ("feature"?) in .NET Framework, which causes corruption of encryptor/
    /// decryptor if a cryptographic exception occurs during encryption/
    /// decryption operation. To correct the problem, re-initialize the class
    /// instance when a cryptographic exception occurs.
    /// </remarks>
    public sealed class RijndaelCryptoProvider : ICryptoProvider, IDisposable {
        #region Private Fields

        // These members will be used to perform encryption and decryption.
        private ICryptoTransform? _encryptor;
        private ICryptoTransform? _decryptor;
        private bool _disposed;

        #endregion

        #region Private Read-Only Fields

        private readonly RijndaelCryptoOptions _options;

        #endregion

        #region Public Constructors

        public RijndaelCryptoProvider()
            : this(RijndaelCryptoOptions.Default) { }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with the key derived from the explicitly specified
        /// parameters.
        /// </summary>
        /// <param name="options">The options.</param>
        public RijndaelCryptoProvider(RijndaelCryptoOptions options) {
            _options = Guard.Against.Null(options, nameof(options));

            // Initialization vector converted to a byte array.
            // Get bytes of initialization vector.
            var ivBytes = Encoding.ASCII.GetBytes(_options.IV);

            // Salt used for password hashing (to generate the key, not during
            // encryption) converted to a byte array.
            // Get bytes of salt (used in hashing).
            var saltValueBytes = Encoding.ASCII.GetBytes(_options.Salt);

            byte[]? keyBytes = null;
            // Generate password, which will be used to derive the key.
            using (var password = new Rfc2898DeriveBytes(_options.Passphrase, saltValueBytes, _options.PasswordIterations, HashAlgorithmName.SHA256)) {
                // Convert key to a byte array adjusting the size from bits to bytes.
                var keySize = _options.KeySize == KeySize.None ? KeySize.Large : _options.KeySize;
                keyBytes = password.GetBytes((int)keySize / 8);
            }

            // Initialize Rijndael key object.
            using var symmetricKey = Aes.Create();
            // If we do not have initialization vector, we cannot use the CBC mode.
            // The only alternative is the ECB mode (which is not as good).
            symmetricKey.Mode = ivBytes.Length == 0 ? CipherMode.ECB : CipherMode.CBC;

            _encryptor = symmetricKey.CreateEncryptor(keyBytes, ivBytes);
            _decryptor = symmetricKey.CreateDecryptor(keyBytes, ivBytes);
        }

        #endregion

        #region Destructors

        /// <summary>
        /// Destructor.
        /// </summary>
        ~RijndaelCryptoProvider() {
            Dispose(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds an array of randomly generated bytes at the beginning of the
        /// array holding original plain text value.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Byte array containing original plain text value.
        /// </param>
        /// <returns>
        /// Either original array of plain text bytes (if salt is not used) or a
        /// modified array containing a randomly generated salt added at the
        /// beginning of the plain text bytes.
        /// </returns>
        private byte[] AddSalt(byte[] plainTextBytes) {
            // Generate the salt.
            var saltBytes = GenerateSalt();

            // Allocate array which will hold salt and plain text bytes.
            var plainTextBytesWithSalt = new byte[plainTextBytes.Length + saltBytes.Length];

            // First, copy salt bytes.
            Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);

            // Append plain text bytes to the salt value.
            Array.Copy(plainTextBytes, 0, plainTextBytesWithSalt, saltBytes.Length, plainTextBytes.Length);

            return plainTextBytesWithSalt;
        }

        /// <summary>
        /// Generates an array holding cryptographically strong bytes.
        /// </summary>
        /// <returns>
        /// Array of randomly generated bytes.
        /// </returns>
        /// <remarks>
        /// Salt size will be defined at random or exactly as specified by the
        /// minSlatLen and maxSaltLen parameters passed to the object constructor.
        /// The first four bytes of the salt array will contain the salt length
        /// split into four two-bit pieces.
        /// </remarks>
        private byte[] GenerateSalt() {
            // We don't have the length, yet.
            // If min and max salt values are the same, it should not be random.
            var saltLength = _options.MinimumSaltSize != _options.MaximumSaltSize
                ? GenerateRandomNumber(_options.MinimumSaltSize, _options.MaximumSaltSize)
                : _options.MinimumSaltSize;

            // Allocate byte array to hold our salt.
            var salt = new byte[saltLength];

            // Populate salt with cryptographically strong bytes.
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(salt);

            // Split salt length (always one byte) into four two-bit pieces and
            // store these pieces in the first four bytes of the salt array.
            salt[0] = (byte)((salt[0] & 0xfc) | (saltLength & 0x03));
            salt[1] = (byte)((salt[1] & 0xf3) | (saltLength & 0x0c));
            salt[2] = (byte)((salt[2] & 0xcf) | (saltLength & 0x30));
            salt[3] = (byte)((salt[3] & 0x3f) | (saltLength & 0xc0));

            return salt;
        }

        private void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _encryptor?.Dispose();
                _decryptor?.Dispose();
            }

            _encryptor = null;
            _decryptor = null;
            _disposed = true;
        }

        private void BlockAccesAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(RijndaelCryptoProvider));

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Generates random integer.
        /// </summary>
        /// <param name="minimunValue">
        /// Min value (inclusive).
        /// </param>
        /// <param name="maximunValue">
        /// Max value (inclusive).
        /// </param>
        /// <returns>
        /// Random integer value between the min and max values (inclusive).
        /// </returns>
        /// <remarks>
        /// This methods overcomes the limitations of .NET Framework's Random
        /// class, which - when initialized multiple times within a very short
        /// period of time - can generate the same "random" number.
        /// </remarks>
        private static int GenerateRandomNumber(int minimunValue, int maximunValue) {
            // We will make up an integer seed from 4 bytes of this array.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            // Convert four random bytes into a positive integer value.
            var seed = ((randomBytes[0] & 0x7f) << 24) |
                (randomBytes[1] << 16) |
                (randomBytes[2] << 8) |
                (randomBytes[3]);

            // Now, this looks more like real randomization.
            // And, calculate a random number.
            return new Random(seed).Next(minimunValue, maximunValue + 1);
        }

        #endregion

        #region ICryptoProvider Members

        /// <inheritdoc />
        public byte[] Encrypt(Stream stream) {
            Guard.Against.Null(stream, nameof(stream));

            BlockAccesAfterDispose();

            if (!stream.CanRead) { throw new InvalidOperationException("Can't read the stream."); }
            if (stream.Length == 0) { return []; }

            // Let's make cryptographic operations thread-safe.
            lock (this) {
                try {
                    // To perform encryption, we must use the Write mode.
                    using var memoryStream = new MemoryStream();
                    using var cryptoStream = new CryptoStream(memoryStream, _encryptor!, CryptoStreamMode.Write);
                    var value = stream.ToByteArray();
                    var valueWithSalt = AddSalt(value);

                    // Start encrypting data.
                    cryptoStream.Write(valueWithSalt, 0, valueWithSalt.Length);
                    // Finish the encryption operation.
                    cryptoStream.FlushFinalBlock();
                    // Return encrypted data.
                    return memoryStream.ToArray();
                } catch (Exception) { Dispose(); throw; }
            }
        }

        /// <inheritdoc />
        public byte[] Decrypt(Stream stream) {
            Guard.Against.Null(stream, nameof(stream));

            BlockAccesAfterDispose();

            if (!stream.CanRead) { throw new InvalidOperationException("Can't read the stream."); }
            if (stream.Length == 0) { return []; }

            byte[] decryptedBytes;
            var decryptedByteCount = 0;
            var saltLength = 0;

            // Let's make cryptographic operations thread-safe.
            lock (this) {
                try {
                    var value = stream.ToByteArray();

                    // Since we do not know how big decrypted value will be, use the same
                    // size as cipher text. Cipher text is always longer than plain text
                    // (in block cipher encryption), so we will just use the number of
                    // decrypted data byte after we know how big it is.
                    decryptedBytes = new byte[value.Length];

                    // To perform decryption, we must use the Read mode.
                    using var memoryStream = new MemoryStream(value);
                    using var cryptoStream = new CryptoStream(memoryStream, _decryptor!, CryptoStreamMode.Read);
                    // Decrypting data and get the count of plain text bytes.
                    decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                } catch (Exception) { Dispose(); throw; }
            }

            // If we are using salt, get its length from the first 4 bytes of plain
            // text data.
            if (_options.MaximumSaltSize > 0 && _options.MaximumSaltSize >= _options.MinimumSaltSize) {
                saltLength = (decryptedBytes[0] & 0x03) |
                             (decryptedBytes[1] & 0x0c) |
                             (decryptedBytes[2] & 0x30) |
                             (decryptedBytes[3] & 0xc0);
            }

            // Allocate the byte array to hold the original plain text (without salt).
            var plainTextBytes = new byte[decryptedByteCount - saltLength];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLength, plainTextBytes, 0, decryptedByteCount - saltLength);

            // Return original plain text value.
            return plainTextBytes;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
